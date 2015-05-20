using Collection_Game_Tool.Divisions;
using Collection_Game_Tool.GameSetup;
using Collection_Game_Tool.Main;
using Collection_Game_Tool.Services;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControlPrizeLevels : UserControl, Listener, Teller
    {
        List<Listener> listenerList = new List<Listener>();
        public PrizeLevels plsObject;
        private int collectionCheck;
        private const double MARGIN = 60;
        private string plsID;

        public UserControlPrizeLevels()
        {
            InitializeComponent();
            plsObject = new PrizeLevels();

            plsID = null;

            //SetsUp the default 2 PrizeLevel
            UserControlPrizeLevel ucpl = new UserControlPrizeLevel();
            ucpl.addListener(this);
            Prizes.Children.Add(ucpl);
            plsObject.addPrizeLevel(ucpl.plObject);
            ucpl.plObject.prizeLevel=1;
            ucpl.CloseButton.IsEnabled = false;
            ucpl.CloseButton.Opacity = 0.0f;

            UserControlPrizeLevel ucpl2 = new UserControlPrizeLevel();
            ucpl2.OuterGrid.Margin = new Thickness(0, Prizes.Children.Count * MARGIN, 0, 0);
            ucpl2.addListener(this);
            Prizes.Children.Add(ucpl2);
            plsObject.addPrizeLevel(ucpl2.plObject);
            ucpl2.plObject.prizeLevel = Prizes.Children.Count;
            ucpl2.CloseButton.IsEnabled = false;
            ucpl2.CloseButton.Opacity = 0.0f;

            this.Loaded += new RoutedEventHandler(UserControlPrizeLevels_Loaded);
            prizeLevelScroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
        }

        private void UserControlPrizeLevels_Loaded(object sender, RoutedEventArgs e)
        {
            Window parentWindow = Window.GetWindow(this.Parent);
            addListener((Window1)parentWindow);
        }

        public void Add_Prize_Level(object sender, RoutedEventArgs e)
        {
            if (plsObject.getNumPrizeLevels() < 12)
            {
                UserControlPrizeLevel ucpl = new UserControlPrizeLevel();
                ucpl.OuterGrid.Margin = new Thickness(0, Prizes.Children.Count * MARGIN, 0, 0);

                ucpl.addListener(this);
                Prizes.Children.Add(ucpl);
                plsObject.addPrizeLevel(ucpl.plObject);
                ucpl.plObject.prizeLevel = Prizes.Children.Count;

                //adds the PrizeLevel to the end
            }

            //Gets rid of any highlight of previously selected PrizeLevel
            for (int i = 0; i < Prizes.Children.Count; i++)
            {
                UserControlPrizeLevel ucpl = (UserControlPrizeLevel)Prizes.Children[i];
                //ucpl.LevelGrid.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("LightGray"));
                ucpl.OuterGrid.Margin = new Thickness(0, i * MARGIN, 0, 0);
                ucpl.plObject.prizeLevel = (i + 1);

                ucpl.CloseButton.IsEnabled = true;
                ucpl.CloseButton.Opacity = 1;
            }

            if (plsObject.getNumPrizeLevels() == 12)
            {
                AddButton.IsEnabled = false;
                AddButton.Opacity = 0.3;
            }

            prizeLevelScroll.ScrollToBottom();
            //Shouts the PrizeLevels object so that they can be analyzed in Divisions
            shout(plsObject);
        }

        public void loadExistingPrizeLevel(PrizeLevel loadedPrizeLevel)
        {
            UserControlPrizeLevel ucpl = new UserControlPrizeLevel();
            ucpl.OuterGrid.Margin = new Thickness(0, Prizes.Children.Count * MARGIN, 0, 0);

            ucpl.addListener(this);
            Prizes.Children.Add(ucpl);
            ucpl.plObject = loadedPrizeLevel;
            ucpl.plObject.initializeListener();
            ucpl.setDataContext();
            ucpl.plObject.prizeLevel = Prizes.Children.Count;
        }

        public void onListen(object pass)
        {
            if (pass is string)
            {
                String parse=(String)pass;
                if (parse.Equals("Update"))
                {
                    List<UserControlPrizeLevel> ucplList = new List<UserControlPrizeLevel>();
                    ucplList = Prizes.Children.Cast<UserControlPrizeLevel>().ToList<UserControlPrizeLevel>();
                    Prizes.Children.Clear();

                    ucplList.Sort();
                    plsObject.sortPrizeLevels();

                    int collectionToShout = 0;
                    int index=0;
                    bool sameFound = false;
                    PrizeLevelConverter plc = new PrizeLevelConverter();
                    
                    for (int i = 0; i < ucplList.Count; i++ )
                    {
                        ucplList[i].LevelGrid.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ADADAD"));
                        ucplList[i].OuterGrid.Margin = new Thickness(0, i * MARGIN, 0, 0);
                        ucplList[i].plObject.prizeLevel = (i + 1);
                        if (ucplList[i].plObject.numCollections>collectionToShout)
                        {
                            collectionToShout = ucplList[i].plObject.numCollections;
                            index=i;
                        }
                        Prizes.Children.Add(ucplList[i]);

                        //This searches for prize levels that are the same
                        if (i != (ucplList.Count - 1))
                        {
                            for (int j = i + 1; j < ucplList.Count; j++)
                            {
                                if (((ucplList[i].plObject.isInstantWin && ucplList[j].plObject.isInstantWin) || (!ucplList[i].plObject.isInstantWin && !ucplList[j].plObject.isInstantWin)) &&
                                    (ucplList[i].plObject.numCollections == ucplList[j].plObject.numCollections) &&
                                    (ucplList[i].plObject.prizeValue == ucplList[j].plObject.prizeValue))
                                {
                                    plsID = ErrorService.Instance.reportWarning("004", new List<string>{
                                        (string)plc.Convert(ucplList[i].plObject.prizeLevel, typeof(string), null, new System.Globalization.CultureInfo("en-us")),
                                        (string)plc.Convert(ucplList[j].plObject.prizeLevel, typeof(string), null, new System.Globalization.CultureInfo("en-us"))
                                    }, plsID);
                                    sameFound = true;
                                }
                            }
                        }
                    }
                    if (!sameFound)
                        ErrorService.Instance.resolveWarning("004", null, plsID);

                    if (collectionCheck < collectionToShout)
                    {
                        plsID=ErrorService.Instance.reportError("004", new List<string>
                        {
                            (string)plc.Convert(ucplList[index].plObject.prizeLevel,typeof(string), null, new System.Globalization.CultureInfo("en-us")),
                            collectionCheck.ToString()
                        }, plsID);
                    }
                    else if (collectionCheck >= collectionToShout)
                    {
                        ErrorService.Instance.resolveError("004", new List<string>
                        {
                            (string)plc.Convert(ucplList[index].plObject.prizeLevel,typeof(string), null, new System.Globalization.CultureInfo("en-us")),
                            collectionCheck.ToString()
                        }, plsID);
                    }
                }
            }
            else if(pass is UserControlPrizeLevel)
            {
                //This removes the PrizeLevel that was just closed
                if (plsObject.getNumPrizeLevels() > 2)
                {
                    UserControlPrizeLevel rem = (UserControlPrizeLevel)pass;

                    int index = -1;
                    for (int i = 0; i < Prizes.Children.Count && index < 0; i++)
                    {
                        UserControlPrizeLevel ucpl = (UserControlPrizeLevel)Prizes.Children[i];
                        if (ucpl == rem)
                            index = i;
                    }

                    rem.plObject = null;
                    Prizes.Children.Remove(rem);
                    plsObject.removePrizeLevel(index);

                    rem = null;

                    for (int i = 0; i < Prizes.Children.Count; i++)
                    {
                        UserControlPrizeLevel ucpl = (UserControlPrizeLevel)Prizes.Children[i];
                        ucpl.LevelGrid.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ADADAD"));
                        ucpl.OuterGrid.Margin = new Thickness(0, i * MARGIN, 0, 0);
                        ucpl.plObject.prizeLevel = (i + 1);

                        if (plsObject.getNumPrizeLevels() == 2)
                        {
                            ucpl.CloseButton.IsEnabled = false;
                            ucpl.CloseButton.Opacity = 0.0f;
                        }
                        else
                        {
                            ucpl.CloseButton.IsEnabled = true;
                            ucpl.CloseButton.Opacity = 1;
                        }
                    }

                    AddButton.IsEnabled = true;
                    AddButton.Opacity = 1;
                }
            }
            //Shouts PrizeLevels object so divisions can analyze it
            shout(plsObject);
        }

        public void shout(object pass)
        {
            foreach (Listener l in listenerList)
            {
                l.onListen(pass);
            }
        }

        public void addListener(Listener list)
        {
            listenerList.Add(list);
        }

        public void setCollectionCheck(int CC)
        {
            collectionCheck = CC;
            ((UserControlPrizeLevel)Prizes.Children[0]).shout("Update");
        }
    }
}
