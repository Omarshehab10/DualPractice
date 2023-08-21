using Common.Enums;
using DAL.Models;
using Lean.Framework.Entities.Integration;
using Lean.Framework.Entities.Provider;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Services.UnitOfWork;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DualPractitionerBE
{
    public class SehaUserService : ISehaIntegrationUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SehaIntegrationConfig _config;
        private readonly IUnitOfWork _unitOfWork;
        protected DualPracticeContext DbContext { get; }

        public SehaUserService(IServiceProvider serviceProvider, IUnitOfWork unitOfWork)
        {
            _httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
            _config = serviceProvider.GetRequiredService<SehaIntegrationConfig>();
            _unitOfWork = unitOfWork;
            DbContext = serviceProvider.GetRequiredService<DualPracticeContext>();
        }

        #region Method :: UpdateIdentityUser
        /// <summary>
        /// Add new user if not exist in database and return user info
        /// </summary>
        /// <param name="userData"></param>
        /// <returns></returns>
        public async Task<UserApiResponse> UpdateIdentityUser(UserData userData)
        {
            try
            {
                if (userData != null)
                {
                    var userEntity = new User()
                    {
                        FullNameAr = userData.UserInfoModel.NameAr,
                        FullNameEn = userData.UserInfoModel.NameEn,
                        ExternalUserId = userData.UserInfoModel.ExternalUserId,
                        EmailAddress = userData.UserInfoModel.Email,
                        UserName = userData.UserInfoModel.Username,
                        UserPermissions = userData.Permissions,
                        OrganizationId = userData.RequestInfoModel.OrgId,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now
                    };
                    var entity = await DbContext.User.AsNoTracking().Where(t => t.ExternalUserId == userEntity.ExternalUserId).FirstOrDefaultAsync();

                    if (entity == null)
                    {                     
                        await DbContext.User.AddAsync(userEntity);
                        await DbContext.SaveChangesAsync();
                        userData.UserInfoModel.Id = userEntity.Id;

                    }
                    else
                    {
                        var updateUser = await DbContext.FindAsync<User>(entity.Id);
                        updateUser.Id = entity.Id;
                        updateUser.FullNameAr = userEntity.FullNameAr;
                        updateUser.FullNameEn = userEntity.FullNameEn;
                        updateUser.EmailAddress = userEntity.EmailAddress;
                        updateUser.UserName = userEntity.UserName;
                        updateUser.ExternalUserId = userEntity.ExternalUserId;
                        updateUser.UserPermissions = userEntity.UserPermissions;
                        updateUser.OrganizationId = userEntity.OrganizationId;
                        updateUser.UpdateDate = DateTime.Now;

                        DbContext.User.Update(updateUser);
                        await DbContext.SaveChangesAsync();

                        userData.UserInfoModel.Id = updateUser.Id;

                    }

                    var model = new UserApiResponse()
                    {
                        Id = userData.UserInfoModel.Id,
                        ExternalUserId = Convert.ToInt32(userEntity.ExternalUserId),
                        Roles = userEntity.UserPermissions,
                        NameAr = userEntity.FullNameAr,
                        NameEn = userEntity.FullNameEn,
                        Email = userEntity.EmailAddress
                    };

                    return model;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"an error occurring while Get Identity User : { ex.Message }");
            }

        }
        #endregion


        #region Method :: UpdateRequestInfoForCurrentUser
        /// <summary>
        ///  Add new Request Info (Organization) if not exist in database and return Request Info
        /// </summary>
        /// <param name="userData"></param>
        /// <returns></returns>
        public async Task<OrganizationApiResponse> UpdateOrgInfoForCurrentUser(UserData userData)
        {
            try
            {
                if (userData != null)
                {
                    var city = userData.RequestInfoModel.CityId == 0 ? null : await UpdateCityInfo(userData.RequestInfoModel);
                    var region = userData.RequestInfoModel.RegionId == 0 ? null : await UpdateRegionInfo(userData.RequestInfoModel);

                    var responce = new Organization
                    {
                        OrganizationId = userData.RequestInfoModel.OrgId,
                        NameAr = userData.RequestInfoModel.NameAr,
                        NameEn = userData.RequestInfoModel.NameEn,
                        RegionNameAr = userData.RequestInfoModel.RegionNameAr,
                        RegionNameEn = userData.RequestInfoModel.RegionNameEn,
                        CityId = (city != null) ? (int)city.CityId : 0,
                        RegionId = (region != null) ? (int)region.RegionId : 0,
                        CityNameAr = userData.RequestInfoModel.CityNameAr,
                        CityNameEn = userData.RequestInfoModel.CityNameEn,
                        CategoryId = userData.RequestInfoModel.CategoryId != null ? (int)userData.RequestInfoModel.CategoryId : 0,
                        SubcategoryId = userData.RequestInfoModel.SubcategoryId != null ? (int)userData.RequestInfoModel.SubcategoryId : 0,
                        SectorId = userData.RequestInfoModel.Sector,
                        ManagmentAreaId = userData.RequestInfoModel.ManagmentAreaId,
                        CreatedBy = userData.UserInfoModel.Id,
                        UpdatedBy = userData.UserInfoModel.Id,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        TypeFlag = userData.RequestInfoModel.TypeFlag
                    };
                    var entity = await DbContext.Organization.AsNoTracking().Where(t => t.OrganizationId == responce.OrganizationId).SingleOrDefaultAsync();

                    if (entity == null)
                    {
                        await DbContext.Organization.AddAsync(responce);
                        await DbContext.SaveChangesAsync();
                        entity = responce;
                    }
                    else
                    {
                        responce.Id = entity.Id;

                        entity.OrganizationId = responce.OrganizationId;
                        entity.NameAr = responce.NameAr;
                        entity.NameEn = responce.NameEn;
                        entity.RegionId = responce.RegionId;
                        entity.RegionNameAr = responce.RegionNameAr;
                        entity.RegionNameEn = responce.RegionNameEn;
                        entity.CityId = responce.CityId;
                        entity.CityNameAr = responce.CityNameAr;
                        entity.CityNameEn = responce.CityNameEn;
                        entity.CategoryId = responce.CategoryId;
                        entity.SubcategoryId = responce.SubcategoryId;
                        entity.SectorId = responce.SectorId;
                        entity.UpdatedBy = userData.UserInfoModel.Id;
                        entity.UpdateDate = DateTime.Now;

                        DbContext.Organization.Update(entity);
                        await DbContext.SaveChangesAsync();
                    }


                    var model = new OrganizationApiResponse()
                    {
                        Id = responce.Id,
                        NameEn = responce.NameEn,
                        NameAr = responce.NameAr,
                        OrgId = Convert.ToInt32(responce.OrganizationId),
                        CityNameEn = responce.CityNameEn,
                        CityNameAr = responce.CityNameAr,
                        CityId = Convert.ToInt32(responce.CityId),
                        RegionNameEn = responce.RegionNameEn,
                        RegionNameAr = responce.RegionNameAr,
                        RegionId = Convert.ToInt32(responce.RegionId),
                        CategoryId = responce.CategoryId,
                        SubcategoryId = responce.SubcategoryId,
                        Sector = Convert.ToInt32(responce.SectorId),
                    };
                    return model;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"an error occurring while Get Identity User : { ex.Message }");
            }
        }

        public async Task UpdateUserDataForCurrentUser(UserData userData)
        {
            userData.UserInfoModel = await UpdateIdentityUser(userData);
            userData.RequestInfoModel = await UpdateOrgInfoForCurrentUser(userData);
        }
        #endregion

        #region Method :: GetRedirectUrlByRole
        /// <summary>
        /// not needed if FE external in diff domian
        /// </summary>
        /// <param name="userRole"></param>
        /// <returns></returns>
        public Tuple<string, string> GetRedirectUrlByRole(UserData userData)
        {
            if (userData != null)
            {
                return new Tuple<string, string>("url", string.Format(_config.FrontendRedirectUrl, "1"));
                //return new Tuple<string, string>("Home", "Home");
            }
            return new Tuple<string, string>("SehaAccount", "AccessDenied");
        }




        #endregion




        #region  Method :: IsValidLogin
        public bool IsValidLogin(UserData userData)
        {
            return userData != null;
            //TODO_khalid check it
        }
        #endregion

        #region method :: GetTestUserInfo, GetTestOrgInfo used in development envirnoment 

        public async Task<UserApiResponse> GetTestUserInfo(int id)
        {
            var s = _httpContextAccessor.HttpContext.Request;
            var user = await _unitOfWork.Repository<User>().Get(x => x.ExternalUserId == id).FirstOrDefaultAsync();
            return user == null ? null : new UserApiResponse
            {
                Id = user.Id,
                NameAr = user.FullNameAr,
                NameEn = user.FullNameEn,
                ExternalUserId = Convert.ToInt32(user.ExternalUserId),
                Roles = user.UserPermissions,
                Email = user.EmailAddress
            };
        }

        public OrganizationApiResponse GetTestOrgInfo(int id)
        {
            // id is UserApiResponse user
            OrganizationApiResponse org;
            if (id <= 10)
            {
                org = new OrganizationApiResponse()
                {
                    Id = 33903,
                    CityId = 1,
                    CityNameAr = "City Name Ar",
                    CityNameEn = "City Name En",
                    NameAr = "Name Ar",
                    NameEn = "Name En",
                    OrgId = 33903,
                    RegionId = 1,
                    RegionNameAr = "Region Name Ar",
                    RegionNameEn = "Region Name En",
                    SubcategoryId = 1,
                    CategoryId = 1,
                    Sector = 1
                };
            }
            else
            {
                org = new OrganizationApiResponse()
                {
                    CityId = 2,
                    CityNameAr = "City2 Name Ar",
                    CityNameEn = "City Name En",
                    NameAr = "Name2 Ar",
                    NameEn = "Name En",
                    OrgId = 2,
                    RegionId = 2,
                    RegionNameAr = "Region2 Name Ar",
                    RegionNameEn = "Region Name En",
                    SubcategoryId = 1,
                    CategoryId = 1,
                    Sector = 1
                };
            }

            return org;
        }
        #endregion


        private async Task<DAL.Models.City> UpdateCityInfo(OrganizationApiResponse orgInfo)
        {
            var city = await DbContext.City.Where(x => x.CityId == orgInfo.CityId).FirstOrDefaultAsync();
            if (city != null)
                return city;


            var newCity = new DAL.Models.City
            {
                CityId = orgInfo.CityId,
                NameAr = orgInfo.CityNameAr,
                NameEn = orgInfo.CityNameEn,
                CreatedDate = DateTime.Now,
                IsDeleted = false,
                RegionId = orgInfo.RegionId,
                LastModificationDate = DateTime.Now
            };

            await _unitOfWork.Repository<DAL.Models.City>().InsertAsync(newCity);
            await _unitOfWork.SaveChangesAsync();

            return newCity;

        }

        private async Task<Region> UpdateRegionInfo(OrganizationApiResponse orgInfo)
        {
            var region = await DbContext.Region.Where(x => x.RegionId == orgInfo.RegionId).FirstOrDefaultAsync();
            if (region != null)
                return region;


            var newRegion = new Region
            {
                RegionId = orgInfo.RegionId,
                NameAr = orgInfo.RegionNameAr,
                NameEn = orgInfo.RegionNameEn,
                CreatedDate = DateTime.Now,
                IsDeleted = false,
                LastModificationDate = DateTime.Now
            };
            await _unitOfWork.Repository<Region>().InsertAsync(newRegion);
            await _unitOfWork.SaveChangesAsync();
            return newRegion;
        }
    }
}

