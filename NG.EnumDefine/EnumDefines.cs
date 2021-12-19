namespace NG.EnumDefine;
public enum AccountType
{
    Local = 1,
    LDAP = 2,
    External = 3,
    OldSystem = 4,
    OldSystemToLocal = 5
}

[Flags]
public enum OtpTypeEnum
{
    LoginByPhone = 1,
    EmailConfirmed = 2,
    PhoneNumberConfirmed = 4,
    OTPByEmail = 8,
    OTPBySMS = 16,
    OTPByApp = 32,
    SmartOTP = 64
}

public enum LoginTypeEnum
{
    Web = 1,
    App = 2,
    Google = 3,
    Facebook = 4,
    Apple = 5
}

public enum GroupTypeEnum
{
    Default = 0,
    Administrator = 1,
    EditorialSecretary = 2, // Thư ký tòa soạn
    SectionSecretary = 3, // Thư ký chuyên mục,
    Reporter = 4, // Phóng viên,
    VnPost = 5, // Đối tác VNPOST
    VnPostCustomer = 6, // khách hàng do vnpost giới thiệu
    Pseudonym = 7, // Bút danh 
}

public enum HttpClientNameEnum
{
    Default, 
    Retry
}

public enum ErrorCodeEnum
{
    NoErrorCode = 0,
    Success = 1,
    Fail = 2,
    InternalExceptions = 500,
    Unauthorized = 401,
    NullRequestExceptions = 501,
    NotExistExceptions = 503,
    PermissionDeny = 403,
    AntiXss = 502
}

public enum StatusEnum
{
    Deleted = -1,
    Active = 1,
    New = 2,
    InActive = 3
}