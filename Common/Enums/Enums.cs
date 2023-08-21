using System.ComponentModel;

namespace Common.Enums
{
    public enum RequestStatus 
    {
        [Description("بانتظار قبول الممارس")]
        New = 2,
        [Description("بإنتظار قبول المنشأة الصحية الحكومية")]
        Waiting_Pu_Accept1 = 3,
        [Description("بإنتظار قبول المستوى الثاني في المنشأة الصحية الحكومية")]
        Waiting_Pu_Accept2 = 4,
        [Description("بإنتظار دفع الرسوم")]
        Waiting_Pr_Pay_2 = 5,
        [Description("الطلب مقبول")]
        Approved = 7,
        [Description("الطلب ملغي من المنشأة الصحية الخاصة")]
        Cancelled_Pr1 = 8,
        [Description("الطلب ملغي من المنشأة الصحية الحكومية")]
        Cancelled_Pu2 = 9,
        [Description("الطلب مرفوض من الممارس")]
        Rejected_Prac1 = 10,
        [Description("الطلب مرفوض من المنشأة الصحية الخاصة")]
        Rejected_Pr2 = 11,
        [Description("الطلب مرفوض من المنشأة الصحية الحكومية")]
        Rejected_Pu3 = 12,
        [Description("الطلب مرفوض من المنشأة  الصحية الحكومية المستوى الثاني")]
        Rejected_Pu4 = 13,
        [Description("الطلب منتهي")]
        Expired = 14,
        [Description("بانتظار قبول الممارس - تجديد")]
        Renewed = 15
    }
    public enum Permissions : int
    {
        AddDualPracticeRequest = 983,
        ApproveOrRejectDPRequest = 984,
        FinalApproveOrRejectDPRequest = 985,       
        Unknown = 0
    }

    public enum Days : int
    {
        [Description("الأحد")]
        Sunday = 1,
        [Description("الأثنين")]
        Monday,
        [Description("الثلاثاء")]
        Tuesday,
        [Description("الأربعاء")]
        Wednesday,
        [Description("الخميس")]
        Thursday,
        [Description("الجمعة")]
        Friday,
        [Description("السبت")]
        Saturday
    }

    public enum ApprovalLevels : int
    {
        OneApproval = 1,
        TwoApprovals,
    }
    public enum parentGovOrganization : int
    {
        Administration = 1,
        Cluster,
    }
    public enum OrganizationType : int
    {
        GovernmentOrganization = 1,
        PrivateOrganization,
    }

    public enum TypeFlags : int
    {
        // صحة
        Root = 1,

        // حساب طبي
        MedicalAccount = 2,

        // حساب اداري
        PartnerAccount = 4,

        // منشأة طبية
        MedicalOrganization = 8,

        // منشأة ادارية
        PartnerAdministrativeOrganization = 16,

        // منشأة تنظيمية طبية
        MedicalOrganizationalOrganization = 32,

        // منشأة تنظيمية ادارية
        PartnerOrganizationalOrganization = 64,

        All = 127
    }
    public enum Nationalitys : int
    {
        [Description("سعودي")]
        Saudi = 113
    }
    public enum AddLogs : int
    {
        [Description("Private Est. user /done with Payment of the license")]
        Private_Est_done_with_Payment = 1,
        [Description("Private Est. user/ reject the request")]
        Private_Est_Reject_with_Payment = 2,
        [Description("Practitioner / Accept request")]
        Practitioner_Accept_request = 3,
        [Description("Practitioner / Reject request")]
        Practitioner_Reject_request = 4,
        [Description("Level 1 Gov Est. / Accept the request")]
        Level1_GovEst_Accept_the_request = 5,
        [Description("Level 1 Gov Est. / Reject the request")]
        Level1_GovEst_Reject_the_request = 6,
        [Description("Level 2 Gov Est. / Accept the request")]
        Level2_GovEst_Accept_the_request = 7,
        [Description("Level 2 Gov Est. / Reject the request")]
        Level2_GovEst_Reject_the_request = 8,
    }

    public enum Organizations : int
    {
        NonMohOrgId = 220644
    }
}
