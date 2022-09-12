using MetalCore.Chores.UI.Setup;
using SimpleInjector;

namespace MetalCore.Chores.UI;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
        var builder = MauiApp.CreateBuilder();
        var container = IoCSetup.SetupIoC(builder.Services);

		builder.Services.AddSimpleInjector(container);

        builder
            .UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});
		
        var app = builder.Build();
		app.Services.UseSimpleInjector(container);

#if DEBUG
		container.Verify();
#endif
		return app;
	}

}