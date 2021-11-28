using NG.Common;
using NG.EnumDefine;

namespace NG.BaseReadModels;
using ProtoBuf;

[ProtoContract]
public class AccountLoginInfo
    {
        [ProtoMember(1)] public string Id { get; set; }
        [ProtoMember(2)] public string Code { get; set; }
        [ProtoMember(3)] public bool IsAdministrator { get; set; }
        [ProtoMember(4)] public HashSet<string> Permissions { get; set; }
        [ProtoMember(5)] public string Token { get; set; }
        [ProtoMember(6)] public string RefToken { get; set; }
        [ProtoMember(7)] public bool OtpVerify { get; set; }
        [ProtoMember(8)] public int Version { get; set; }

        [ProtoMember(9)] public string ClientId { get; set; }

        //[ProtoMember(10)] public string ExternalId { get; set; }
        [ProtoMember(11)] public AccountType AccountType { get; set; }
        [ProtoMember(12)] public bool RememberMe { get; set; }
        [ProtoMember(13)] public bool TwoFactorEnabled { get; set; }
        [ProtoMember(14)] public string PhoneNumber { get; set; }
        [ProtoMember(15)] public string Email { get; set; }
        [ProtoMember(16)] public string FullName { get; set; }
        [ProtoMember(17)] public string AvatarUrl { get; set; }

        [ProtoMember(23)] public string[] DepartmentIdsInTree { get; set; }
        [ProtoMember(24)] public string[] DepartmentIds { get; set; }
        [ProtoMember(22)] public string CurrentCompanyId { get; set; }
        [ProtoMember(25)] public string[] CompaniesIds { get; set; }
        [ProtoMember(26)] public string[] CategoriesIds { get; set; }

        [ProtoMember(29)] public string LoginUid { get; set; }
        [ProtoMember(30)] public OtpTypeEnum OtpType { get; set; }
        [ProtoMember(31)] public OtpTypeEnum OtpTypeDefault { get; set; }
        [ProtoMember(32)] public DateTime InitDate { get; set; }
        [ProtoMember(33)] public int OTPSendCount { get; set; }
        [ProtoMember(34)] public int MinuteExpire { get; set; }
        [ProtoMember(35)] public bool OtpCMSVerify { get; set; }
        [ProtoMember(36)] public LoginTypeEnum LoginType { get; set; }
        [ProtoMember(37)] public string CurrentDomain { get; set; }
        [ProtoMember(38)] public string[] Domains { get; set; }
        [ProtoMember(39)] public string CurrentWebsiteId { get; set; }
        [ProtoMember(40)] public string[] WebsiteIds { get; set; }
        [ProtoMember(41)] public GroupTypeEnum GroupType { get; set; }
        [ProtoMember(42)] public bool PhoneNumberConfirmed { get; set; }
        [ProtoMember(43)] public string[] GroupAdmins { get; set; }

        public string EmailOrPhoneNumber
        {
            get
            {
                if (!string.IsNullOrEmpty(Email))
                {
                    return Email;
                }

                if (!string.IsNullOrEmpty(PhoneNumber))
                {
                    return PhoneNumber;
                }

                return Code;
            }
        }

        public string DisplayName
        {
            get
            {
                string displayName = FullName;
                if (string.IsNullOrEmpty(displayName))
                {
                    displayName = Email;
                }

                if (string.IsNullOrEmpty(displayName))
                {
                    displayName = PhoneNumber;
                }

                return displayName;
            }
        }

        public string ShortName => CommonUtility.UserGetShortName(FullName);

        public string[] CategoryIdPermissions(string[] categoryIds)
        {
            if (categoryIds == null || categoryIds.Length <= 0)
            {
                return new string[0];
            }

            if (IsAdministrator)
            {
                return categoryIds;
            }

            if (CategoriesIds == null || CategoriesIds.Length <= 0)
            {
                return new string[0];
            }

            return CategoriesIds.Intersect(categoryIds).ToArray();
        }

        [ProtoMember(44)] public DateTime? ImpersonateBeginDate { get; set; }
        [ProtoMember(45)] public DateTime? ImpersonateEndDate { get; set; }
        [ProtoMember(46)] public bool IsImpersonate { get; set; }
        [ProtoMember(47)] public string CurrentLanguageId { get; set; }
    }