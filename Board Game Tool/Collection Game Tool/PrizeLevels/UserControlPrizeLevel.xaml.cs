using Collection_Game_Tool.GameSetup;
using Collection_Game_Tool.Main;
using Collection_Game_Tool.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Collection_Game_Tool.PrizeLevels
{
    /// <summary>
    /// Interaction logic for UserControlPrizeLevel.xaml
    /// </summary>
    public partial class UserControlPrizeLevel : UserControl, Teller, IComparable
    {
        public PrizeLevel plObject { get; set; }
        List<Listener> listenerList = new List<Listener>();
        private string ucplID = null;

        public UserControlPrizeLevel()
        {
            InitializeComponent();
            //Sets up the model object and the data context for the binding in the xaml
            plObject = new PrizeLevel();
            setDataContext();
            plObject.isInstantWin = false;
            plObject.numCollections = 1;
            plObject.prizeValue = 0;

            this.Loaded += new RoutedEventHandler(MainView_Loaded);
        }

        public void setDataContext()
        {
            Level.DataContext = plObject;
            TextBoxValue.DataContext = plObject;
            CollectionBoxValue.DataContext = plObject;
            InstantWinCheckBox.DataContext = plObject;
            BonusGameCheckBox.DataContext = plObject;
        }

        void MainView_Loaded(object sender, RoutedEventArgs e)
        {
            Window parentWindow = Window.GetWindow(this.Parent);
            //Listens to the PrizeLevels window
            plObject.AddListener((Window1)parentWindow);
        }

        public void Close_Prize_Level(object sender, RoutedEventArgs e)
        {
            //Shouts itself to PrizeLevels so PrizeLevels can close the individual PrizeLevel
            Shout(this);
            Shout("Update");
            MainWindowModel.Instance.verifyDivisions();
            MainWindowModel.Instance.verifyNumTiles();
        }

        private void boxChangedEventHandler(object sender, RoutedEventArgs args)
        {
            //Shouts update to PrizeLevels so PrizeLevels can update the order of individual PrizeLevel if needed
            boxSelected();
        }

        private void TextBox_Focus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            textBox.SelectAll();
            boxSelected();
        }

        private void TextBox_MouseCapture(object sender, MouseEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            textBox.SelectAll();
        }

        public void Shout(object pass)
        {
            foreach (Listener ucpls in listenerList)
            {
                ucpls.OnListen(pass);
            }
        }

        //Highlights box so that user can see it is currently being used
        private void boxSelected()
        {
            validateMyself();
            double set = 0.0;
            if(double.TryParse(TextBoxValue.Text, out set))
                this.plObject.prizeValue = set;
            Shout("Update");
            LevelGrid.Background = Brushes.Yellow;
        }

        public void AddListener(Listener list)
        {
            listenerList.Add(list);
        }

        private void textBoxValue_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !TextBoxTextAllowed(e.Text);
            Shout("Update");
        }

        private void textBoxCollection_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !CollectionBoxTextAllowed(e.Text);
            Shout("Update");
        }

        private bool TextBoxTextAllowed(string p)
        {
            Shout("Update");
            return Array.TrueForAll<Char>(p.ToCharArray(), delegate(Char c) { return Char.IsDigit(c) || Char.IsControl(c) || c.Equals('.'); });
        }

        private bool CollectionBoxTextAllowed(string p)
        {
            Shout("Update");
            return Array.TrueForAll<Char>(p.ToCharArray(), delegate(Char c) { return Char.IsDigit(c) || Char.IsControl(c); });
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
                return 1;

            UserControlPrizeLevel compare = obj as UserControlPrizeLevel;

            return compare.plObject.prizeValue.CompareTo(plObject.prizeValue);
        }

        private void Text_Changed(object sender, TextChangedEventArgs e)
        {
            boxSelected();
        }

        private void validateMyself()
        {
            if (Validation.GetHasError(CollectionBoxValue))
            {
                RangeRule rr = new RangeRule();
                ValidationResult vr = rr.Validate(CollectionBoxValue.Text, new CultureInfo("en-US", false));

                PrizeLevelConverter plc = new PrizeLevelConverter();
                if (vr.Equals(new ValidationResult(false, "Illegal characters")))
                {
                    ucplID = ErrorService.Instance.ReportError("005", new List<string>{
                        (string)plc.Convert(plObject.prizeLevel)
                    }, ucplID);
                }
                else if (vr.Equals(new ValidationResult(false, "Please enter a number in the given range.")))
                {
                    ucplID = ErrorService.Instance.ReportError("006", new List<string>{
                        (string)plc.Convert(plObject.prizeLevel),
                        "0",
                        "10"
                    }, ucplID);
                }
                else if(vr.Equals(new ValidationResult(false, "Cannot be nothing")))
                {
                    ucplID = ErrorService.Instance.ReportError("008", new List<string>{
                        (string)plc.Convert(plObject.prizeLevel)
                    },
                    ucplID);
                }
            }
            else
            {
                ErrorService.Instance.ResolveError("005", ucplID);
                ErrorService.Instance.ResolveError("006", ucplID);
                ErrorService.Instance.ResolveError("008", ucplID);
            }
            Shout("Update");
            MainWindowModel.Instance.verifyDivisions();
            MainWindowModel.Instance.verifyNumTiles();
        }
    }
}
