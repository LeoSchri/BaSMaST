using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using System.Globalization;

namespace BaSMaST_V3
{
    public partial class Config
    {
        public static List<CostumAttribute> Attributes { get; set; }
        public TextBox AddAttributeName { get; set; }
        public MainWindow.ComboBox AttributeType { get; set; }
        public Button AcceptAttribute { get; set; }
        public Button CancelAttribute { get; set; }
        public CheckBox AllowsNull { get; set; }

        public TypeName AttributesFor { get; set; }

        public void ApplyAttributeSetter()
        {
            Apply.Visibility = Visibility.Collapsed;

            AttributesDock.Width = ContentPanel.ActualWidth;

            AddAttributeBtn.PreviewMouseLeftButtonDown += AddCostumAttribute;

            var AddAttributeImage = new Image();
            AddAttributeImage.Source = new BitmapImage(new Uri(Helper.GetIcon("Add")));
            AddAttributeImage.Height = AddAttributeBtn.ActualHeight * 0.6;
            AddAttributeImage.MaxWidth = AddAttributeBtn.ActualWidth * 0.6;
            AddAttributeBtn.Content = AddAttributeImage;

            var AttributeLabel = new Label();
            AttributeLabel.Content = TextCatalog.GetName("New Attribute");
            AttributeLabel.Margin = new Thickness(10, 0, 2, 5);
            AttributeLabel.Height = AppSettings_User.FontSize * 2.25;
            AttributeLabel.Background = null;
            AttributeLabel.Foreground = ( Brush ) bc.ConvertFrom(ColorSchema.Gray.Color1);
            AttributeLabel.FontSize = AppSettings_User.FontSize;
            AttributeLabel.FontFamily = new FontFamily(AppSettings_Static.Font1);
            AttributeLabel.VerticalContentAlignment = VerticalAlignment.Center;
            AttributeLabel.HorizontalContentAlignment = HorizontalAlignment.Left;
            AttributeLabel.HorizontalAlignment = HorizontalAlignment.Left;
            AttributeLabel.VerticalAlignment = VerticalAlignment.Top;
            AttributeLabel.Padding = new Thickness(2, 0, 0, 0);
            AttributeLabel.BorderThickness = new Thickness(0);
            AttributeLabel.BorderBrush = null;
            AttributeLabel.Height = AppSettings_User.FontSize * 1.75;
            AttributeLabel.SetValue(DockPanel.DockProperty, Dock.Top);
            AddAttributeDock.Children.Add(AttributeLabel);

            AddAttributeName = new TextBox();
            AddAttributeName.Margin = new Thickness(10, 0, 2, 5);
            AddAttributeName.Height = AppSettings_User.FontSize * 1.75;
            AddAttributeName.Background = ( Brush ) bc.ConvertFrom(ColorSchema.Primary2);
            AddAttributeName.Foreground = ( Brush ) bc.ConvertFrom(AppSettings_User.ColorSchema.Color1);
            AddAttributeName.FontSize = AppSettings_User.FontSize;
            AddAttributeName.FontFamily = new FontFamily(AppSettings_Static.Font2);
            AddAttributeName.VerticalContentAlignment = VerticalAlignment.Top;
            AddAttributeName.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            AddAttributeName.HorizontalAlignment = HorizontalAlignment.Stretch;
            AddAttributeName.VerticalAlignment = VerticalAlignment.Top;
            AddAttributeName.Padding = new Thickness(2, 0, 0, 0);
            AddAttributeName.BorderThickness = new Thickness(1);
            AddAttributeName.BorderBrush = null;
            AddAttributeName.Style = ( Style ) AddAttributeName.FindResource("ComboBoxTextBox");

            CancelAttribute = new Button();
            CancelAttribute.Margin = new Thickness(2, 0, 2, 5);
            CancelAttribute.Background = ( Brush ) bc.ConvertFrom(ColorSchema.Gray.Color3);
            CancelAttribute.HorizontalAlignment = HorizontalAlignment.Right;
            CancelAttribute.VerticalAlignment = VerticalAlignment.Top;
            CancelAttribute.Height = AppSettings_User.FontSize * 1.75;
            CancelAttribute.Width = CancelAttribute.Height;
            CancelAttribute.BorderThickness = new Thickness(0);
            CancelAttribute.Style = ( Style ) CancelAttribute.FindResource("PopupButtonRed");
            CancelAttribute.SetValue(DockPanel.DockProperty, Dock.Right);
            CancelAttribute.PreviewMouseLeftButtonDown += RemoveAttribute;
            AddAttributeDock.Children.Add(CancelAttribute);

            var CancelAttributeImage = new Image();
            CancelAttributeImage.Source = new BitmapImage(new Uri(Helper.GetIcon("Cancel")));
            CancelAttributeImage.Height = CancelAttribute.Height * 0.6;
            CancelAttributeImage.MaxWidth = CancelAttribute.Width * 0.6;
            CancelAttribute.Content = CancelAttributeImage;

            AcceptAttribute = new Button();
            AcceptAttribute.Margin = new Thickness(2, 0, 2, 5);
            AcceptAttribute.Background = ( Brush ) bc.ConvertFrom(ColorSchema.Gray.Color3);
            AcceptAttribute.HorizontalAlignment = HorizontalAlignment.Right;
            AcceptAttribute.VerticalAlignment = VerticalAlignment.Top;
            AcceptAttribute.Height = AppSettings_User.FontSize * 1.75;
            AcceptAttribute.Width = AcceptAttribute.Height;
            AcceptAttribute.BorderThickness = new Thickness(0);
            AcceptAttribute.Style = ( Style ) AcceptAttribute.FindResource("PopupButtonGreen");
            AcceptAttribute.SetValue(DockPanel.DockProperty, Dock.Right);
            AcceptAttribute.PreviewMouseLeftButtonDown += NewAttribute;
            AddAttributeDock.Children.Add(AcceptAttribute);

            var AcceptAttributeImage = new Image();
            AcceptAttributeImage.Source = new BitmapImage(new Uri(Helper.GetIcon("Tick")));
            AcceptAttributeImage.Height = AcceptAttribute.Height * 0.6;
            AcceptAttributeImage.MaxWidth = AcceptAttribute.Width * 0.6;
            AcceptAttribute.Content = AcceptAttributeImage;

            AttributeType= new MainWindow.ComboBox("AttributeType", AddAttributeDock, 140, 35, VerticalAlignment.Top, HorizontalAlignment.Stretch, Dock.Right, new Thickness(0), new List<string>() { "Text",TextCatalog.GetName("Whole number"),TextCatalog.GetName("Double"),TextCatalog.GetName("Boolean"), TextCatalog.GetName("Date"),TextCatalog.GetName("ListOfString"), TextCatalog.GetName("ListOfInt"), TextCatalog.GetName("ListOfDouble") }, "Text", false, false);

            AllowsNull = new CheckBox();
            AllowsNull.Content = TextCatalog.GetName("AllowsNull");
            AllowsNull.Margin = new Thickness(2, 0, 2, 5);
            AllowsNull.Background = ( Brush ) bc.ConvertFrom(ColorSchema.Primary2);
            AllowsNull.BorderBrush = ( Brush ) bc.ConvertFrom(ColorSchema.Primary2);
            AllowsNull.HorizontalAlignment = HorizontalAlignment.Right;
            AllowsNull.Foreground = ( Brush ) bc.ConvertFrom(AppSettings_User.ColorSchema.Color1);
            AllowsNull.FontSize = AppSettings_User.FontSize;
            AllowsNull.FontFamily = new FontFamily(AppSettings_Static.Font2);
            AllowsNull.VerticalAlignment = VerticalAlignment.Top;
            AllowsNull.Height = AppSettings_User.FontSize * 1.75;
            AllowsNull.MinWidth = 50;
            AllowsNull.BorderThickness = new Thickness(1);
            AllowsNull.SetValue(DockPanel.DockProperty, Dock.Right);
            AddAttributeDock.Children.Add(AllowsNull);

            AddAttributeDock.Children.Add(AddAttributeName);

            LoadAttributes();
        }

        private void LoadAttributes(bool WithSorting = true)
        {
            Attributes = new List<CostumAttribute>();
            CustomAttributesStack.Children.Clear();

            if (AttributesFor == TypeName.Character)
            {
                if (Character.Attributes != null && Character.Attributes.Any())
                {
                    SortAttributes();
                    Character.Attributes.ForEach(a =>
                    {
                        new CostumAttribute(a, CustomAttributesStack);
                    });
                }
            }
            else if (AttributesFor == TypeName.MainCharacter)
            {
                if (MainCharacter.Attributes != null && MainCharacter.Attributes.Any())
                {
                    SortAttributes();
                    MainCharacter.Attributes.ForEach(a =>
                    {
                        new CostumAttribute(a, CustomAttributesStack);
                    });
                }
            }
        }

        private void SortAttributes()
        {
            var attributes = new List<Attribute>();
            if (AttributesFor == TypeName.Character)
            {
                attributes = Character.Attributes;
            }
            else if (AttributesFor == TypeName.MainCharacter)
            {
                attributes = MainCharacter.Attributes;
            }
            attributes.Sort(delegate (Attribute x,Attribute y)
                {
                    int a = x.Name.CompareTo(y.Name);
                    if (a == 0)
                        a = x.Type.ToString().CompareTo(y.Type.ToString());
                    return a;
                });
        }

        private void AddCostumAttribute(object sender, MouseButtonEventArgs e)
        {
            AddAttributeDock.Visibility = Visibility.Visible;
            AddAttributeBtn.Visibility = Visibility.Collapsed;
        }

        private void RemoveAttribute(object sender, MouseButtonEventArgs e)
        {
            AddAttributeName.Text = string.Empty;
            AddAttributeDock.Visibility = Visibility.Collapsed;
            AddAttributeBtn.Visibility = Visibility.Visible;
        }

        private void NewAttribute(object sender, MouseButtonEventArgs e)
        {
            if (!ValidateAttribute())
                return;

            var type = typeof(string);
            if (AttributeType.ComboBoxTextBox.Text == TextCatalog.GetName("Whole number"))
            {
                type = typeof(int);
            }
            else if (AttributeType.ComboBoxTextBox.Text == TextCatalog.GetName("Double"))
            {
                type = typeof(double);
            }
            else if (AttributeType.ComboBoxTextBox.Text == TextCatalog.GetName("Boolean"))
            {
                type = typeof(bool);
            }
            else if (AttributeType.ComboBoxTextBox.Text == TextCatalog.GetName("Date"))
            {
                type = typeof(DateTime);
            }
            else if (AttributeType.ComboBoxTextBox.Text == TextCatalog.GetName("ListOfString"))
            {
                type = typeof(List<string>);
            }
            else if (AttributeType.ComboBoxTextBox.Text == TextCatalog.GetName("ListOfInt"))
            {
                type = typeof(List<int>);
            }
            else if (AttributeType.ComboBoxTextBox.Text == TextCatalog.GetName("ListOfDouble"))
            {
                type = typeof(List<double>);
            }

            var attribute = new Attribute(AddAttributeName.Text.Replace(" ",""), type, AppSettings_User.CurrentProject,AttributesFor,AllowsNull.IsChecked==true?true:false);

            new CostumAttribute(attribute, CustomAttributesStack);

            if(AttributesFor == TypeName.Character)
            {
                if (Character.Attributes == null)
                    Character.Attributes = new List<Attribute>();
                Character.AddAttributes(new List<Attribute>() { attribute },TypeName.Character,AppSettings_User.CurrentProject.CharacterManager,true);
            }
            else if (AttributesFor == TypeName.MainCharacter)
            {
                if (MainCharacter.Attributes == null)
                    MainCharacter.Attributes = new List<Attribute>();
                MainCharacter.AddAttributes(new List<Attribute>() { attribute }, TypeName.MainCharacter, AppSettings_User.CurrentProject.CharacterManager, false);
            }

            AddAttributeName.Text = string.Empty;
            AddAttributeDock.Visibility = Visibility.Collapsed;
            AddAttributeBtn.Visibility = Visibility.Visible;
        }

        private static void RemoveAffectedAttribute(object sender, MouseButtonEventArgs e)
        {
            var costumAttribute = Attributes.Find(a => a.Remove == sender);
            Popup.ShowWindow(Helper.MakeDeletionText("Attribute", costumAttribute.AffectedAttribute.Name), TextCatalog.GetName("Confirm deletion"), PopupButtons.YesNo, PopupType.Delete, costumAttribute.AffectedAttribute);
        }

        private bool ValidateAttribute()
        {
            var _dataIsValid = true;

            foreach (TextBox c in Helper.FindWindowChildren<TextBox>(AddAttributeDock))
            {
                if (string.IsNullOrEmpty(c.Text))
                {
                    c.BorderBrush = ( Brush ) bc.ConvertFrom(ColorSchema.Red.Color1);
                    _dataIsValid = false;
                }
                else
                {
                    c.BorderBrush = null;
                }
            }

            return _dataIsValid;
        }

        public class CostumAttribute
        { 
            public DockPanel Container { get; set; }
            public Label AttributeName { get; set; }
            public Label AttributeType { get; set; }
            public Label AllowsNull { get; set; }
            public Button Remove { get; set; }

            public Attribute AffectedAttribute { get; private set; }

            public CostumAttribute(Attribute attribute,StackPanel parent)
            {
                CreateAttribute(attribute, parent);
                Attributes.Add(this);
            }

            public void CreateAttribute(Attribute attribute, StackPanel parent)
            {
                Container = new DockPanel();
                Container.Margin = new Thickness(5, 0, 0, 0);
                AffectedAttribute = attribute;

                AttributeName = new Label();
                AttributeName.Content = AffectedAttribute.Name;
                AttributeName.Margin = new Thickness(2, 0, 2, 5);
                AttributeName.Height = AppSettings_User.FontSize * 1.75;
                AttributeName.Background = ( Brush ) bc.ConvertFrom(ColorSchema.Primary2);
                AttributeName.Foreground = ( Brush ) bc.ConvertFrom(AppSettings_User.ColorSchema.Color1);
                AttributeName.FontSize = AppSettings_User.FontSize;
                AttributeName.FontFamily = new FontFamily(AppSettings_Static.Font2);
                AttributeName.VerticalContentAlignment = VerticalAlignment.Center;
                AttributeName.HorizontalContentAlignment = HorizontalAlignment.Center;
                AttributeName.HorizontalAlignment = HorizontalAlignment.Stretch;
                AttributeName.VerticalAlignment = VerticalAlignment.Top;
                AttributeName.Padding = new Thickness(2, 0, 0, 0);
                AttributeName.BorderThickness = new Thickness(0);
                AttributeName.BorderBrush = ( Brush ) bc.ConvertFrom(ColorSchema.Primary2);
                AttributeName.Style = ( Style ) AttributeName.FindResource("RoundedLabel");
                AttributeName.Height = AppSettings_User.FontSize * 1.75;

                Remove = new Button();
                Remove.Style = ( Style ) Remove.FindResource("PopupButtonRed");
                Remove.Margin = new Thickness(5, 0, 10, 5);
                Remove.Background = null;
                Remove.BorderBrush = null;
                Remove.Height = AppSettings_User.FontSize * 1.75;
                Remove.Width = Remove.Height;
                Remove.HorizontalAlignment = HorizontalAlignment.Right;
                Remove.VerticalAlignment = VerticalAlignment.Top;
                Remove.HorizontalContentAlignment = HorizontalAlignment.Center;
                Remove.VerticalContentAlignment = VerticalAlignment.Center;
                Remove.SetValue(DockPanel.DockProperty, Dock.Right);
                Remove.BorderThickness = new Thickness(0);
                Remove.PreviewMouseLeftButtonDown += RemoveAffectedAttribute;

                var RemoveImage = new Image();
                RemoveImage.Source = new BitmapImage(new Uri(Helper.GetIcon("Remove")));
                RemoveImage.Width = Remove.Width * 0.6;
                Remove.Content = RemoveImage;
                Container.Children.Add(Remove);

                AllowsNull = new Label();
                AllowsNull.Content = AffectedAttribute.AllowsNull?TextCatalog.GetName("AllowsNull"): TextCatalog.GetName("NotNull");
                AllowsNull.Margin = new Thickness(2, 0, 2, 5);
                AllowsNull.Height = AppSettings_User.FontSize * 1.75;
                AllowsNull.MinWidth = 70;
                AllowsNull.Background = ( Brush ) bc.ConvertFrom(ColorSchema.Primary2);
                AllowsNull.Foreground = ( Brush ) bc.ConvertFrom(AppSettings_User.ColorSchema.Color1);
                AllowsNull.FontSize = AppSettings_User.FontSize;
                AllowsNull.FontFamily = new FontFamily(AppSettings_Static.Font2);
                AllowsNull.VerticalContentAlignment = VerticalAlignment.Center;
                AllowsNull.HorizontalContentAlignment = HorizontalAlignment.Center;
                AllowsNull.HorizontalAlignment = HorizontalAlignment.Stretch;
                AllowsNull.VerticalAlignment = VerticalAlignment.Top;
                AllowsNull.Padding = new Thickness(2, 0, 0, 0);
                AllowsNull.BorderThickness = new Thickness(0);
                AllowsNull.BorderBrush = ( Brush ) bc.ConvertFrom(ColorSchema.Primary2);
                AllowsNull.Style = ( Style ) AllowsNull.FindResource("RoundedLabel");
                AllowsNull.SetValue(DockPanel.DockProperty, Dock.Right);
                Container.Children.Add(AllowsNull);

                var type = "Text";
                if(attribute.Type == typeof(int))
                {
                    type = TextCatalog.GetName("Whole number");
                }
                else if (attribute.Type == typeof(double))
                {
                    type = TextCatalog.GetName("Double");
                }
                else if (attribute.Type == typeof(bool))
                {
                    type = TextCatalog.GetName("Boolean");
                }
                else if (attribute.Type == typeof(DateTime))
                {
                    type = TextCatalog.GetName("Date");
                }
                else if (attribute.Type == typeof(List<string>))
                {
                    type = TextCatalog.GetName("ListOfString");
                }
                else if (attribute.Type == typeof(List<int>))
                {
                    type = TextCatalog.GetName("ListOfInt");
                }
                else if (attribute.Type == typeof(List<double>))
                {
                    type = TextCatalog.GetName("ListOfDouble");
                }

                AttributeType = new Label();
                AttributeType.Content = type;
                AttributeType.Margin = new Thickness(2, 0, 2, 5);
                AttributeType.Height = AppSettings_User.FontSize * 1.75;
                AttributeType.MinWidth = 70;
                AttributeType.Background = ( Brush ) bc.ConvertFrom(ColorSchema.Primary2);
                AttributeType.Foreground = ( Brush ) bc.ConvertFrom(AppSettings_User.ColorSchema.Color1);
                AttributeType.FontSize = AppSettings_User.FontSize;
                AttributeType.FontFamily = new FontFamily(AppSettings_Static.Font2);
                AttributeType.VerticalContentAlignment = VerticalAlignment.Center;
                AttributeType.HorizontalContentAlignment = HorizontalAlignment.Center;
                AttributeType.HorizontalAlignment = HorizontalAlignment.Stretch;
                AttributeType.VerticalAlignment = VerticalAlignment.Top;
                AttributeType.Padding = new Thickness(2, 0, 0, 0);
                AttributeType.BorderThickness = new Thickness(0);
                AttributeType.BorderBrush = ( Brush ) bc.ConvertFrom(ColorSchema.Primary2);
                AttributeType.Style = ( Style ) AttributeType.FindResource("RoundedLabel");
                AttributeType.SetValue(DockPanel.DockProperty, Dock.Right);
                Container.Children.Add(AttributeType);

                Container.Children.Add(AttributeName);
                parent.Children.Add(Container);
            }
        }
    }
}
