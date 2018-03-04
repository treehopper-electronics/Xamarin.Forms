using System;
using System.ComponentModel;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform;

namespace Xamarin.Forms
{
	[RenderWith(typeof(_EditorRenderer))]
	public class Editor : InputView, IEditorController, IFontElement, ITextElement, IElementConfiguration<Editor>
	{
		public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(Editor), null, BindingMode.TwoWay, propertyChanged: (bindable, oldValue, newValue)
			=> ((Editor)bindable).TextChanged?.Invoke(bindable, new TextChangedEventArgs((string)oldValue, (string)newValue)));

		public static readonly BindableProperty FontFamilyProperty = FontElement.FontFamilyProperty;

		public static readonly BindableProperty FontSizeProperty = FontElement.FontSizeProperty;

		public static readonly BindableProperty FontAttributesProperty = FontElement.FontAttributesProperty;

		public static readonly BindableProperty TextColorProperty = TextElement.TextColorProperty;


		public static readonly BindableProperty EditorSizeOptionProperty = BindableProperty.Create(nameof(SizeOption), typeof(EditorSizeOption), typeof(Editor), defaultValue: EditorSizeOption.Default, propertyChanged: (bindable, oldValue, newValue)
			=> OnSizeOptionChanged((Editor)bindable, (EditorSizeOption)oldValue, (EditorSizeOption)newValue));

		readonly Lazy<PlatformConfigurationRegistry<Editor>> _platformConfigurationRegistry;


		public EditorSizeOption SizeOption
		{
			get { return (EditorSizeOption)GetValue(EditorSizeOptionProperty); }
			set { SetValue(EditorSizeOptionProperty, value); }
		}

		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}

		public Color TextColor
		{
			get { return (Color)GetValue(TextElement.TextColorProperty); }
			set { SetValue(TextElement.TextColorProperty, value); }
		}

		public FontAttributes FontAttributes
		{
			get { return (FontAttributes)GetValue(FontAttributesProperty); }
			set { SetValue(FontAttributesProperty, value); }
		}

		public string FontFamily
		{
			get { return (string)GetValue(FontFamilyProperty); }
			set { SetValue(FontFamilyProperty, value); }
		}

		[TypeConverter(typeof(FontSizeConverter))]
		public double FontSize
		{
			get { return (double)GetValue(FontSizeProperty); }
			set { SetValue(FontSizeProperty, value); }
		}

		void IFontElement.OnFontFamilyChanged(string oldValue, string newValue)
		{
		}

		void IFontElement.OnFontSizeChanged(double oldValue, double newValue)
		{
		}

		void IFontElement.OnFontChanged(Font oldValue, Font newValue)
		{
		}

		double IFontElement.FontSizeDefaultValueCreator() =>
			Device.GetNamedSize(NamedSize.Default, (Editor)this);

		void IFontElement.OnFontAttributesChanged(FontAttributes oldValue, FontAttributes newValue)
		{
		}

		public event EventHandler Completed;

		public event EventHandler<TextChangedEventArgs> TextChanged;

		public Editor()
		{
			_platformConfigurationRegistry = new Lazy<PlatformConfigurationRegistry<Editor>>(() => new PlatformConfigurationRegistry<Editor>(this));
		}


		static SizeRequest NullSizeRequest = new SizeRequest(new Size(-1,-1), new Size(-1,-1));
		Size cachedResult = new Size(-1, -1);
		SizeRequest cachedSizeRequest = NullSizeRequest;
		SizeRequest sizeRequestToUse = NullSizeRequest;

		private void OnTextChanged(object sender, TextChangedEventArgs e)
		{
			if (SizeOption == EditorSizeOption.AutoSizeToTextChanges)
			{
				var requestSize = base.OnMeasure(cachedResult.Width, cachedResult.Height);
				if(!requestSize.Equals(cachedSizeRequest))
				{
					sizeRequestToUse = requestSize;
					this.InvalidateMeasureInternal(InvalidationTrigger.MeasureChanged);
				}
			}
		}


		protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
		{
			if (SizeOption == EditorSizeOption.Default)
			{
				return base.OnMeasure(widthConstraint, heightConstraint);
			}

			SizeRequest returnValue;
			if (!sizeRequestToUse.Equals(NullSizeRequest))
			{
				returnValue = sizeRequestToUse;
				sizeRequestToUse = NullSizeRequest;
			}
			else
			{
				returnValue = base.OnMeasure(widthConstraint, heightConstraint);
				cachedResult = new Size(widthConstraint, heightConstraint);
			}

			cachedSizeRequest = returnValue;

			return returnValue;
		}
		 

		public IPlatformElementConfiguration<T, Editor> On<T>() where T : IConfigPlatform
		{
			return _platformConfigurationRegistry.Value.On<T>();
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SendCompleted()
			=> Completed?.Invoke(this, EventArgs.Empty);

		void ITextElement.OnTextColorPropertyChanged(Color oldValue, Color newValue)
		{
		}

		private static void OnSizeOptionChanged(Editor bindable, EditorSizeOption oldValue, EditorSizeOption newValue)
		{
			if (newValue == EditorSizeOption.AutoSizeToTextChanges)
			{
				bindable.TextChanged += bindable.OnTextChanged;
			}
			else
			{
				bindable.TextChanged -= bindable.OnTextChanged;
			}
		}
	}
}