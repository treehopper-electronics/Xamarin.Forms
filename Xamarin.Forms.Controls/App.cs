using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.PlatformConfiguration.WindowsSpecific;

namespace Xamarin.Forms.Controls
{

	public class App : Application
	{
		public const string AppName = "XamarinFormsControls";
		static string s_insightsKey;

		// ReSharper disable once InconsistentNaming
		public static int IOSVersion = -1;

		public static List<string> AppearingMessages = new List<string>();

		static Dictionary<string, string> s_config;
		readonly ITestCloudService _testCloudService;

		public const string DefaultMainPageId = "ControlGalleryMainPage";

		public App()
		{
			_testCloudService = DependencyService.Get<ITestCloudService>();
			
			SetMainPage(CreateDefaultMainPage());

			//// Uncomment to verify that there is no gray screen displayed between the blue splash and red MasterDetailPage.
			//SetMainPage(new Bugzilla44596SplashPage(() =>
			//{
			//	var newTabbedPage = new TabbedPage();
			//	newTabbedPage.Children.Add(new ContentPage { BackgroundColor = Color.Red, Content = new Label { Text = "yay" } });
			//	MainPage = new MasterDetailPage
			//	{
			//		Master = new ContentPage { Title = "Master", BackgroundColor = Color.Red },
			//		Detail = newTabbedPage
			//	};
			//}));

			//// Uncomment to verify that there is no crash when switching MainPage from MDP inside NavPage
			//SetMainPage(new Bugzilla45702());
		}


		public class SomeViewModel
		{
			public System.Windows.Input.ICommand Command { get; set; }
		}

		public Page CreateDefaultMainPage()
		{
			StackLayout container = new StackLayout()
			{
				BackgroundColor = Color.Purple
			};


			StackLayout layout = new StackLayout()
			{
				BackgroundColor = Color.Pink,
				HeightRequest = 200
			};

			var editor = new Editor()
			{
				MaxLength = 2000,
				BackgroundColor = Color.Green,
				MinimumHeightRequest = 10,
				SizeOption = EditorSizeOption.AutoSizeToTextChanges
			};
			 

			container.Children.Add(layout);
			layout.Children.Add(editor);

			return new ContentPage()
			{
				Content = container
			};




			/////*
			////Label someLabel = new Label();

			////someLabel.Effects.Add(new Issues.Bugzilla58406._58406Effect());

			////return new ContentPage()
			////{
			////	Content = someLabel
			////};
			////*/
			//////return new ContentPage()
			//////{
			//////	Content = new ImageButton()
			//////	{
			//////		Source = "coffee.png",
			//////		//ContentLayout = new Button.ButtonContentLayout(Button.ButtonContentLayout.ImagePosition.Right, 100),
			//////		//Text = "Whatever",

			//////	}
			//////};

			//ImageButton button = new ImageButton() { Source = "coffee.png" };

			////button.CornerRadius = 12;
			////button.BorderWidth = 12;
			////button.BorderColor = Color.Green;


			//Button button2 = new Button();
			//button2.Image = "coffee.png";

			////button2.CornerRadius = 12;
			////button2.BorderWidth = 12;
			////button2.BorderColor = Color.Green;

			//Button button3 = new Button() { Text = "Kill the Image" };
			//StackLayout layout = new StackLayout();
			//Image image = new Image() { Source = "coffee.png" };
			//WeakReference reference = new WeakReference(image);
			//WeakReference fileImageSource = new WeakReference(image.Source);

			//Button button4 = new Button() { Text = "Go somewhere" };
			//button4.Clicked += async (_, __) =>
			//{
			//	await App.Current.MainPage.Navigation.PushAsync(
			//		new ContentPage()
			//		{
			//			Content = new ImageButton() { Source = "coffee.png" }
			//		}
			//	);
			//};

			//button.Command = new Command(() =>
			//{
			//	button.CornerRadius /= 2;
			//	button.BorderWidth *= 0.9;
			//	button.BorderColor = Color.Pink;
			//});

			//button2.Clicked += (x, y) =>
			//{
			//	button.IsEnabled = !button.IsEnabled;

			//	if (!button.IsEnabled)
			//	{

			//		((FileImageSource)image.Source).File = "bank.png";
			//	}
			//	else
			//	{
			//		((FileImageSource)image.Source).File = "coffee.png";
			//	}

			//	GC.Collect();
			//	GC.Collect(1);
			//};

			//button3.Clicked += (x, y) =>
			//{
			//	if (image != null)
			//	{
			//		layout.Children.Remove(image);
			//		image = null;
			//	}

			//	GC.Collect();
			//	GC.Collect(1);
			//	button3.Text = $"{reference.IsAlive} {fileImageSource.IsAlive}";
			//};

			//var stateGroups = new VisualStateGroupList();
			//var visualStateGroup = new VisualStateGroup { Name = "CommonStates" };
			//var normalState = new VisualState { Name = "Normal" };
			//normalState.Setters.Add(new Setter() { Property = ImageButton.SourceProperty, Value = "coffee.png" });

			//var disabledState = new VisualState { Name = "Disabled" };
			//disabledState.Setters.Add(new Setter() { Property = ImageButton.SourceProperty, Value = "bank.png" });

			//var pressedState = new VisualState { Name = "Pressed" };
			//pressedState.Setters.Add(new Setter() { Property = ImageButton.SourceProperty, Value = "calculator.png" });

			//var focusedState = new VisualState { Name = "Focused" };
			//focusedState.Setters.Add(new Setter() { Property = ImageButton.SourceProperty, Value = "Fruits.jpg" });


			//visualStateGroup.States.Add(normalState);
			//visualStateGroup.States.Add(disabledState);
			//visualStateGroup.States.Add(focusedState);
			//visualStateGroup.States.Add(pressedState);

			//stateGroups.Add(visualStateGroup);

			//VisualStateManager.SetVisualStateGroups(button, stateGroups);

			////button.FlowDirection = FlowDirection.RightToLeft;
			////button2.FlowDirection = FlowDirection.RightToLeft;
			////button3.FlowDirection = FlowDirection.RightToLeft;
			////button4.FlowDirection = FlowDirection.LeftToRight;




			//List<Tuple<string, string, double>> people = new List<Tuple<string, string, double>>();


			//for (int i = 0; i < 200; i++)
			//{
			//	if ((i % 2) == 0)
			//		people.Add((new Tuple<string, string, double>("coffee.png", "Purple", i % 20)));
			//	else
			//		people.Add((new Tuple<string, string, double>("bank.png", "Green", i % 20)));


			//}

			////setup listview test
			//var personDataTemplate = new DataTemplate(() =>
			//{
			//	var grid = new StackLayout() { Orientation = StackOrientation.Horizontal };
			//	var nameLabel = new ImageButton();
			//	var nameLabel2 = new Button();
			//	var imageTest = new Image();

			//	nameLabel.SetBinding(ImageButton.SourceProperty, "Item1");
			//	nameLabel.SetBinding(ImageButton.BorderColorProperty, "Item2");
			//	nameLabel.SetBinding(ImageButton.BorderWidthProperty, "Item3");


			//	nameLabel2.SetBinding(Button.ImageProperty, "Item1");
			//	nameLabel2.SetBinding(Button.BorderColorProperty, "Item2");
			//	nameLabel2.SetBinding(Button.BorderWidthProperty, "Item3");


			//	imageTest.SetBinding(Image.SourceProperty, "Item1");
			//	//nameLabel2.SetBinding(Image.BorderColorProperty, "Item2");
			//	//nameLabel2.SetBinding(Image.BorderWidthProperty, "Item3");

			//	grid.Children.Add(nameLabel);
			//	//grid.Children.Add(nameLabel2);
			//	grid.Children.Add(imageTest);

			//	return new ViewCell { View = grid };
			//});

			//layout.Children.Add(button);
			////layout.Children.Add(button3);
			////layout.Children.Add(button4);
			//layout.Children.Add(image);
			//layout.Children.Add(button2);

			////layout.Children.Add(new ListView { ItemsSource = people, ItemTemplate = personDataTemplate });


			//return new NavigationPage(new ContentPage()
			//{
			//	Content = layout
			//});

			//return new Issues.Bugzilla37625();

			//var layout = new StackLayout { BackgroundColor = Color.Red };
			//layout.Children.Add(new Label { Text = "This is master Page" });
			//var master = new ContentPage { Title = "Master", Content = layout, BackgroundColor = Color.SkyBlue };
			//master.On<iOS>().SetUseSafeArea(true);
			//return new MasterDetailPage
			//{
			//	AutomationId = DefaultMainPageId,
			//	Master = master,
			//	Detail = CoreGallery.GetMainPage()
			//};
		}

		protected override void OnAppLinkRequestReceived(Uri uri)
		{
			var appDomain = "http://" + AppName.ToLowerInvariant() + "/";

			if (!uri.ToString().ToLowerInvariant().StartsWith(appDomain))
				return;

			var url = uri.ToString().Replace(appDomain, "");

			var parts = url.Split('/');
			if (parts.Length == 2)
			{
				var isPage = parts[0].Trim().ToLower() == "gallery";
				if (isPage)
				{
					string page = parts[1].Trim();
					var pageForms = Activator.CreateInstance(Type.GetType(page));

					var appLinkPageGallery = pageForms as AppLinkPageGallery;
					if (appLinkPageGallery != null)
					{
						appLinkPageGallery.ShowLabel = true;
						(MainPage as MasterDetailPage)?.Detail.Navigation.PushAsync((pageForms as Page));
					}
				}
			}

			base.OnAppLinkRequestReceived(uri);
		}

		public static Dictionary<string, string> Config
		{
			get
			{
				if (s_config == null)
					LoadConfig();

				return s_config;
			}
		}

		public static ContentPage MenuPage { get; set; }

		public void SetMainPage(Page rootPage)
		{
			MainPage = rootPage;
		}

		static Assembly GetAssembly(out string assemblystring)
		{
			assemblystring = typeof(App).AssemblyQualifiedName.Split(',')[1].Trim();
			var assemblyname = new AssemblyName(assemblystring);
			return Assembly.Load(assemblyname);
		}

		static void LoadConfig()
		{
			s_config = new Dictionary<string, string>();

			string keyData = LoadResource("controlgallery.config").Result;
			string[] entries = keyData.Split("\n\r".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			foreach (string entry in entries)
			{
				string[] parts = entry.Split(':');
				if (parts.Length < 2)
					continue;

				s_config.Add(parts[0].Trim(), parts[1].Trim());
			}
		}

		static async Task<string> LoadResource(string filename)
		{
			string assemblystring;
			Assembly assembly = GetAssembly(out assemblystring);

			Stream stream = assembly.GetManifestResourceStream($"{assemblystring}.{filename}");
			string text;
			using (var reader = new StreamReader(stream))
				text = await reader.ReadToEndAsync();
			return text;
		}

		public bool NavigateToTestPage(string test)
		{
			try
			{
				// Create an instance of the main page
				var root = CreateDefaultMainPage();

				// Set up a delegate to handle the navigation to the test page
				EventHandler toTestPage = null;

				toTestPage = delegate(object sender, EventArgs e) 
				{
					Current.MainPage.Navigation.PushModalAsync(TestCases.GetTestCases());
					TestCases.TestCaseScreen.PageToAction[test]();
					Current.MainPage.Appearing -= toTestPage;
				};

				// And set that delegate to run once the main page appears
				root.Appearing += toTestPage;

				SetMainPage(root);

				return true;
			}
			catch (Exception ex) 
			{
				Log.Warning("UITests", $"Error attempting to navigate directly to {test}: {ex}");

			}

			return false;
		}
		
		public void Reset()
		{
			SetMainPage(CreateDefaultMainPage());
		}
	}
}