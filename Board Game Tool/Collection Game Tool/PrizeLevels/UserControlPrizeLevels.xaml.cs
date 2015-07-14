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
        private const double MARGIN = 60;
        private string plsID;

        public UserControlPrizeLevels()
        {
            InitializeComponent();

            plsID = null;

            this.Loaded += new RoutedEventHandler(UserControlPrizeLevels_Loaded);
            prizeLevelScroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            prizeLevelCounterLabel.Content = Prizes.Children.Count;
        }

        public void AddDefaultPrizeLevels()
        {
            //SetsUp the default 2 PrizeLevel
            UserControlPrizeLevel ucpl = new UserControlPrizeLevel();
            ucpl.AddListener(this);
            Prizes.Children.Add(ucpl);
            MainWindowModel.Instance.PrizeLevelsModel.addPrizeLevel(ucpl.plObject);
            ucpl.plObject.prizeLevel = 1;
            ucpl.CloseButton.IsEnabled = false;
            ucpl.CloseButton.Opacity = 0.0f;

            UserControlPrizeLevel ucpl2 = new UserControlPrizeLevel();
            ucpl2.OuterGrid.Margin = new Thickness(0, Prizes.Children.Count * MARGIN, 0, 0);
            ucpl2.AddListener(this);
            Prizes.Children.Add(ucpl2);
            MainWindowModel.Instance.PrizeLevelsModel.addPrizeLevel(ucpl2.plObject);
            ucpl2.plObject.prizeLevel = Prizes.Children.Count;
            ucpl2.CloseButton.IsEnabled = false;
            ucpl2.CloseButton.Opacity = 0.0f;
        }

        private void UserControlPrizeLevels_Loaded(object sender, RoutedEventArgs e)
        {
            Window parentWindow = Window.GetWindow(this.Parent);
            AddListener((MainWindow)parentWindow);
        }

        public void Add_Prize_Level(object sender, RoutedEventArgs e)
        {
            if (MainWindowModel.Instance.PrizeLevelsModel.getNumPrizeLevels() < 12)
            {
                UserControlPrizeLevel ucpl = new UserControlPrizeLevel();
                ucpl.OuterGrid.Margin = new Thickness(0, Prizes.Children.Count * MARGIN, 0, 0);

                ucpl.AddListener(this);
                Prizes.Children.Add(ucpl);
                MainWindowModel.Instance.PrizeLevelsModel.addPrizeLevel(ucpl.plObject);
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

            if (MainWindowModel.Instance.PrizeLevelsModel.getNumPrizeLevels() == 12)
            {
                AddButton.IsEnabled = false;
                AddButton.Opacity = 0.3;
            }

            prizeLevelScroll.ScrollToBottom();
            prizeLevelCounterLabel.Content = Prizes.Children.Count;
            MainWindowModel.Instance.VerifyNumTiles();
            MainWindowModel.Instance.VerifyDivisions();
            //Shouts the PrizeLevels object so that they can be analyzed in Divisions
            Shout(MainWindowModel.Instance.PrizeLevelsModel);
        }

        public void loadExistingPrizeLevel(PrizeLevel loadedPrizeLevel)
        {
            UserControlPrizeLevel ucpl = new UserControlPrizeLevel();
            ucpl.OuterGrid.Margin = new Thickness(0, Prizes.Children.Count * MARGIN, 0, 0);

            ucpl.AddListener(this);
            Prizes.Children.Add(ucpl);
            ucpl.plObject = loadedPrizeLevel;
            ucpl.plObject.initializeListener();
            ucpl.setDataContext();
            ucpl.plObject.prizeLevel = Prizes.Children.Count;
            prizeLevelCounterLabel.Content = Prizes.Children.Count;
        }

        public void checkLoadedPrizeLevels()
        {
            //Gets rid of any highlight of previously selected PrizeLevel
            for (int i = 0; i < Prizes.Children.Count; i++)
            {
                UserControlPrizeLevel ucpl = (UserControlPrizeLevel)Prizes.Children[i];
                //ucpl.LevelGrid.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("LightGray"));
                ucpl.OuterGrid.Margin = new Thickness(0, i * MARGIN, 0, 0);
                ucpl.plObject.prizeLevel = (i + 1);

                if (Prizes.Children.Count > 2)
                {
                    ucpl.CloseButton.IsEnabled = true;
                    ucpl.CloseButton.Opacity = 1;
                }
                else
                {
                    ucpl.CloseButton.IsEnabled = false;
                    ucpl.CloseButton.Opacity = 0;
                }
            }
        }

        public void OnListen(object pass)
        {
            if (pass is string)
            {
                String parse=(String)pass;
                if (parse.Equals("Update"))
                {
                    MainWindowModel.Instance.PrizeLevelsModel.calculateTotalCollections();

                    List<UserControlPrizeLevel> ucplList = new List<UserControlPrizeLevel>();
                    ucplList = Prizes.Children.Cast<UserControlPrizeLevel>().ToList<UserControlPrizeLevel>();
                    Prizes.Children.Clear();

                    ucplList.Sort();
                    MainWindowModel.Instance.PrizeLevelsModel.sortPrizeLevels();

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
                                    plsID = ErrorService.Instance.ReportWarning("004", new List<string>{
                                        (string)plc.Convert(ucplList[i].plObject.prizeLevel),
                                        (string)plc.Convert(ucplList[j].plObject.prizeLevel)
                                    }, plsID);
                                    sameFound = true;
                                }
                            }
                        }
                    }
                    if (!sameFound)
                        ErrorService.Instance.ResolveWarning("004", plsID);
                }
             
            }
            else if(pass is UserControlPrizeLevel)
            {
                //This removes the PrizeLevel that was just closed
                if (MainWindowModel.Instance.PrizeLevelsModel.getNumPrizeLevels() > 2)
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
                    MainWindowModel.Instance.PrizeLevelsModel.removePrizeLevel(index);

                    rem = null;

                    for (int i = 0; i < Prizes.Children.Count; i++)
                    {
                        UserControlPrizeLevel ucpl = (UserControlPrizeLevel)Prizes.Children[i];
                        ucpl.LevelGrid.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ADADAD"));
                        ucpl.OuterGrid.Margin = new Thickness(0, i * MARGIN, 0, 0);
                        ucpl.plObject.prizeLevel = (i + 1);

                        if (MainWindowModel.Instance.PrizeLevelsModel.getNumPrizeLevels() == 2)
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
            prizeLevelCounterLabel.Content = Prizes.Children.Count;

            //Shouts PrizeLevels object so divisions can analyze it
            Shout(MainWindowModel.Instance.PrizeLevelsModel);
        }

        public void Shout(object pass)
        {
            foreach (Listener l in listenerList)
            {
                l.OnListen(pass);
            }
        }

        public void AddListener(Listener list)
        {
            listenerList.Add(list);
        }
    }
}
