namespace ConsentManagementProviderLib
{
    public interface ICmpSdk : ISpSdk
    {
        bool UseGDPR { get; }
        bool UseCCPA { get; }
        bool UseUSNAT { get; }
        bool UseIOS14 { get; }
    }
}