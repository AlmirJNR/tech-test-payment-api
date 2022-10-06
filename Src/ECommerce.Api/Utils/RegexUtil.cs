using System.Text.RegularExpressions;

namespace ECommerce.Api.Utils;

public static class RegexUtil
{
    private static readonly Regex CpfRegex = new(@"[0-9]{3}\.[0-9]{3}\.[0-9]{3}\-[0-9]{2}");
    private static readonly Regex EmailRegex = new(@"[\w0-9]+\@[\w0-9]+(?:\.[\w]+)+$");
    private static readonly Regex TelephoneRegex = new(@"\+{1}[0-9]{1,4}\([0-9]{2,3}\)[0-9]{4,5}\-[0-9]{4}");

    public static bool IsCpfValid(string cpf) => CpfRegex.Match(cpf).Success;
    public static bool IsEmailValid(string email) => EmailRegex.Match(email).Success;
    public static bool IsTelephoneValid(string telephone) => TelephoneRegex.Match(telephone).Success;
}