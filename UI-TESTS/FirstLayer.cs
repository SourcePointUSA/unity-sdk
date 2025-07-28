namespace UnityAppiumTests
{
    public abstract class FirstLayer
    {
        public abstract string textViewPath { get; }
        public abstract string textViewPathES { get; }
        public abstract string showOptionsPath { get; }
        public abstract string rejectAllPath { get; }
        public abstract string acceptAllPath { get; }
        public abstract WebDriverWait wait { get; }
		#pragma warning disable CS8618
        public DriverHelper driverHelper;
		#pragma warning restore CS8618

        public void pressAcceptAll() => driverHelper.pressButton(acceptAllPath, textViewPath, false, true);
        public void pressRejectAll() => driverHelper.pressButton(rejectAllPath, textViewPath, false, true);
    } 
}
