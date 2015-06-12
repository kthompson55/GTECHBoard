﻿using System;
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
using System.ComponentModel;
using Collection_Game_Tool.PrizeLevels;

namespace Collection_Game_Tool.Divisions
{
    /// <summary>
    /// Interaction logic for PrizeLevelBox.xaml
    /// </summary>
    [Serializable]
    public partial class PrizeLevelBox : UserControl
    {
        public LevelBox levelModel { get; set; }
        private DivisionUC division { get; set; }

        public PrizeLevelBox(DivisionUC div, LevelBox box)
        {
            InitializeComponent();
            division = div;
            levelModel = box;
            levelBox.DataContext = levelModel;
            prizeLevelLabel.DataContext = levelModel;
        }

        /// <summary>
        /// Event for when a prize level in a division is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void levelBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            levelModel.switchIsSelected();
            division.updateInfo();
        }

    }
}
