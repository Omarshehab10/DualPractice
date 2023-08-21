using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;


namespace DAL.Models
{
    public partial class DualPracticeContext : DbContext
    {
        public readonly IConfiguration configuration;

        public DualPracticeContext(IConfiguration _configuration, DbContextOptions options)
            : base(options)
        {
            configuration = _configuration;
        }

        public DualPracticeContext(DbContextOptions options)
           : base(options)
        {
        }

        public virtual DbSet<ActionTypeLookup> ActionTypeLookup { get; set; }
        public virtual DbSet<AdministrativeArea> AdministrativeArea { get; set; }
        public virtual DbSet<AdminstrativeOrgLookup> AdminstrativeOrgLookup { get; set; }
        public virtual DbSet<City> City { get; set; }
        public virtual DbSet<Comment> Comment { get; set; }
        public virtual DbSet<DaySchedule> DaySchedule { get; set; }
        public virtual DbSet<DpRequest> DpRequest { get; set; }
        public virtual DbSet<Logs> Logs { get; set; }
        public virtual DbSet<MedicalOrganizationSubCategory> MedicalOrganizationSubCategory { get; set; }
        public virtual DbSet<Nationality> Nationality { get; set; }
        public virtual DbSet<NonMohOrganizationLookup> NonMohOrganizationLookups { get; set; }
        public virtual DbSet<Organization> Organization { get; set; }
        public virtual DbSet<PaymentTransactionDetail> PaymentTransactionDetail { get; set; }
        public virtual DbSet<Practioner> Practioner { get; set; }
        public virtual DbSet<Region> Region { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<RejectionReasonsLookup> RejectionReasonsLookups { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=10.22.129.157;Database=DualPractice;User ID=DualPracticeUser;Password=04JBNijYz2i;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ActionTypeLookup>(entity =>
            {
                entity.ToTable("ActionType_Lookup");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Description).HasMaxLength(200);
            });

            modelBuilder.Entity<AdministrativeArea>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.NameAr)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.NameEn)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<AdminstrativeOrgLookup>(entity =>
            {
                entity.ToTable("Adminstrative_Org_Lookup");

                entity.HasIndex(e => e.SehaOrganizationId)
                    .HasName("IX_Adminstrative_Org_Lookup")
                    .IsUnique();

                entity.HasOne(d => d.SehaOrganization)
                    .WithOne(p => p.AdminstrativeOrgLookup)
                    .HasPrincipalKey<Organization>(p => p.OrganizationId)
                    .HasForeignKey<AdminstrativeOrgLookup>(d => d.SehaOrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Adminstrative_Org_Lookup_Organization");
            });

            modelBuilder.Entity<City>(entity =>
            {
                entity.HasIndex(e => e.CityId)
                    .HasName("IX_City")
                    .IsUnique();

                entity.Property(e => e.CityId).IsRequired();

                entity.Property(e => e.NameAr)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.NameEn)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.ToTable("Comment");

                entity.Property(e => e.DprequestId).HasColumnName("DPRequestId");

                entity.Property(e => e.Text)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.Comment)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Comment_User");

                entity.HasOne(d => d.Dprequest)
                    .WithMany(p => p.Comment)
                    .HasForeignKey(d => d.DprequestId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Comment_DP_Request");

                entity.HasOne(d => d.RejectionReason)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.RejectionReasonId)
                    .HasConstraintName("FK_Comment_Rejection_Reasons_Lookup");
            });

            modelBuilder.Entity<DaySchedule>(entity =>
            {
                entity.ToTable("Day_Schedule");

                entity.Property(e => e.DprequestId).HasColumnName("DPRequestId");

                entity.Property(e => e.TotalHours).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.Dprequest)
                    .WithMany(p => p.DaySchedule)
                    .HasForeignKey(d => d.DprequestId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Day_Schedule_DP_Request");
            });

            modelBuilder.Entity<DpRequest>(entity =>
            {
                entity.ToTable("DP_Request");

                entity.Property(e => e.ApprovedReportUrl).HasMaxLength(500);

                entity.Property(e => e.NormalizedServiceCode).HasMaxLength(20);

                entity.Property(e => e.PractionerEmailAddress).HasMaxLength(50);

                entity.Property(e => e.PractionerIdNumber)
                    .IsRequired()
                    .HasMaxLength(15);

                entity.Property(e => e.PractionerMobileNumber).HasMaxLength(20);

                entity.Property(e => e.ServiceCodeCode).HasColumnName("ServiceCode_Code");

                entity.Property(e => e.ServiceCodeDate)
                    .HasColumnName("ServiceCode_Date")
                    .HasColumnType("date");

                entity.Property(e => e.ServiceCodePrefix)
                    .HasColumnName("ServiceCode_Prefix")
                    .HasMaxLength(10);

                entity.Property(e => e.TotalWeeklyHours).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.DpRequestCreatedByNavigation)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_DP_Request_User");

                entity.HasOne(d => d.PractionerIdNumberNavigation)
                    .WithMany(p => p.DpRequest)
                    .HasPrincipalKey(p => p.NationalId)
                    .HasForeignKey(d => d.PractionerIdNumber)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DP_Request_Practioner");

                entity.HasOne(d => d.PractionerMainOrg)
                    .WithMany(p => p.DpRequestPractionerMainOrg)
                    .HasForeignKey(d => d.PractionerMainOrgId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DP_Request_Organization1");

                entity.HasOne(d => d.RequestingOrg)
                    .WithMany(p => p.DpRequestRequestingOrg)
                    .HasForeignKey(d => d.RequestingOrgId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DP_Request_Organization");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.DpRequestUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("FK_DP_Request_DP_User1");
            });

            modelBuilder.Entity<Logs>(entity =>
            {
                entity.Property(e => e.NormalizedServiceCode).HasMaxLength(20);

                entity.HasOne(d => d.ActionTypeNavigation)
                    .WithMany(p => p.Logs)
                    .HasForeignKey(d => d.ActionType)
                    .HasConstraintName("FK_Logs_ActionType_Lookup");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Logs)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Logs_User");
            });

            modelBuilder.Entity<MedicalOrganizationSubCategory>(entity =>
            {
                entity.ToTable("Medical_Organization_SubCategory");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.NameAr)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.NameEn)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Nationality>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Isocode)
                    .HasColumnName("ISOCode")
                    .HasMaxLength(50);

                entity.Property(e => e.NameAr)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.NameEn)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<NonMohOrganizationLookup>(entity =>
            {
                entity.ToTable("Non-MOH_Organization_Lookup");

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.Region).HasMaxLength(50);
            });

            modelBuilder.Entity<Organization>(entity =>
            {
                entity.HasIndex(e => e.OrganizationId)
                    .HasName("IX_Organization")
                    .IsUnique();

                entity.Property(e => e.CityNameAr).HasMaxLength(50);

                entity.Property(e => e.CityNameEn).HasMaxLength(50);

                entity.Property(e => e.LicenseNumber).HasMaxLength(50);

                entity.Property(e => e.Mohid).HasColumnName("MOHId");

                entity.Property(e => e.NameAr).HasMaxLength(255);

                entity.Property(e => e.NameEn).HasMaxLength(255);

                entity.Property(e => e.NhciorganizationId).HasColumnName("NHCIOrganizationId");

                entity.Property(e => e.OrganizationId).HasColumnName("organizationId");

                entity.Property(e => e.RegionNameAr).HasMaxLength(50);

                entity.Property(e => e.RegionNameEn).HasMaxLength(50);

                entity.HasOne(d => d.City)
                    .WithMany(p => p.Organization)
                    .HasPrincipalKey(p => p.CityId)
                    .HasForeignKey(d => d.CityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Organization_City");

                entity.HasOne(d => d.ManagmentArea)
                    .WithMany(p => p.Organization)
                    .HasForeignKey(d => d.ManagmentAreaId)
                    .HasConstraintName("FK_Organization_AdministrativeArea");

                entity.HasOne(d => d.Region)
                    .WithMany(p => p.Organization)
                    .HasPrincipalKey(p => p.RegionId)
                    .HasForeignKey(d => d.RegionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Organization_Region");

                entity.HasOne(d => d.SehaAdministrativeOrganization)
                    .WithMany(p => p.InverseSehaAdministrativeOrganization)
                    .HasPrincipalKey(p => p.OrganizationId)
                    .HasForeignKey(d => d.SehaAdministrativeOrganizationId)
                    .HasConstraintName("FK_Organization_Organization");

                entity.HasOne(d => d.Subcategory)
                    .WithMany(p => p.Organization)
                    .HasForeignKey(d => d.SubcategoryId)
                    .HasConstraintName("FK_Organization_Medical_Organization_SubCategory");
            });

            modelBuilder.Entity<PaymentTransactionDetail>(entity =>
            {
                entity.ToTable("Payment_Transaction_Detail");

                entity.Property(e => e.DpprocessId).HasColumnName("DPProcessId");

                entity.Property(e => e.DprequestId).HasColumnName("DPRequestId");

                entity.Property(e => e.ServiceCode)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.TransactionCode)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.Dprequest)
                    .WithMany(p => p.PaymentTransactionDetail)
                    .HasForeignKey(d => d.DprequestId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Payment_Transactin_Detail_DP_Request");
            });

            modelBuilder.Entity<Practioner>(entity =>
            {
                entity.HasIndex(e => e.NationalId)
                    .IsUnique();

                entity.Property(e => e.DateOfBirthG).HasColumnName("DateOfBirth_G");

                entity.Property(e => e.DateOfBirthH)
                    .HasColumnName("DateOfBirth_H")
                    .HasMaxLength(20);

                entity.Property(e => e.FirstNameAr).HasMaxLength(50);

                entity.Property(e => e.FirstNameEn).HasMaxLength(50);

                entity.Property(e => e.FullNameAr).HasMaxLength(255);

                entity.Property(e => e.FullNameEn).HasMaxLength(255);

                entity.Property(e => e.Gender)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.LastNameAr).HasMaxLength(50);

                entity.Property(e => e.LastNameEn).HasMaxLength(50);

                entity.Property(e => e.LicenseNumber).HasMaxLength(100);

                entity.Property(e => e.NationalId)
                    .IsRequired()
                    .HasMaxLength(15);

                entity.Property(e => e.ScfhsCategoryAr).HasMaxLength(50);

                entity.Property(e => e.ScfhsCategoryEn).HasMaxLength(50);

                entity.Property(e => e.ScfhsRegistrationNumber).HasMaxLength(50);

                entity.Property(e => e.SecondNameAr).HasMaxLength(50);

                entity.Property(e => e.SecondNameEn).HasMaxLength(50);

                entity.Property(e => e.SpecialtyCode).HasMaxLength(50);

                entity.Property(e => e.SpecialtyNameAr).HasMaxLength(100);

                entity.Property(e => e.SpecialtyNameEn).HasMaxLength(100);

                entity.Property(e => e.ThirdNameAr).HasMaxLength(50);

                entity.Property(e => e.ThirdNameEn).HasMaxLength(50);

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.Practioner)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_Practioner_Users");

                entity.HasOne(d => d.NationalityNavigation)
                    .WithMany(p => p.Practioner)
                    .HasForeignKey(d => d.Nationality)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Practioner_Nationality");
            });

            modelBuilder.Entity<Region>(entity =>
            {
                entity.HasIndex(e => e.RegionId)
                    .HasName("IX_Region")
                    .IsUnique();

                entity.Property(e => e.NameAr)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.NameEn)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.RegionId).IsRequired();
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.EmailAddress).HasMaxLength(50);

                entity.Property(e => e.FullNameAr)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.FullNameEn)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.IdiqamaNumber)
                    .HasColumnName("IDIqamaNumber")
                    .HasMaxLength(15);

                entity.Property(e => e.UserName).HasMaxLength(255);

                entity.Property(e => e.UserPermissions).HasMaxLength(255);
            });

            modelBuilder.Entity<RejectionReasonsLookup>(entity =>
            {
                entity.ToTable("Rejection_Reasons_Lookup");

                entity.Property(e => e.ReasonAr).HasMaxLength(500);

                entity.Property(e => e.ReasonEn)
                    .HasMaxLength(500);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
