using Collection_Game_Tool.Divisions;
using Collection_Game_Tool.GameSetup;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Collection_Game_Tool.Main
{
    [Serializable]
    class ProjectData
    {
		public PrizeLevels.PrizeLevels SavedPrizeLevels { get; set; }
		public GameSetupModel SavedGameSetup { get; set; }
		public DivisionsModel SavedDivisions { get; set; }

        [NonSerialized]
        private string _projectFileName;
		[NonSerialized]
		public bool isProjectSaved;
        [NonSerialized]
        private const string _DEFAULT_EXT = ".bggproj";
        [NonSerialized]
        private const string _FILTER = "Board Game Generator Project (" + _DEFAULT_EXT + ")|*" + _DEFAULT_EXT;

        public ProjectData()
        {
            _projectFileName = null;
            isProjectSaved = false;
        }

        public void SaveProject(GameSetupModel gsObject, PrizeLevels.PrizeLevels plsObject, DivisionsModel divisionsList)
        {
            if (isProjectSaved)
            {
                SavedGameSetup = gsObject;
                SavedPrizeLevels = plsObject;
                SavedDivisions = divisionsList;

                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(_projectFileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
                formatter.Serialize(stream, this);
                stream.Close();
            }
            else
            {
                SaveProjectAs(gsObject, plsObject, divisionsList);
            }
        }

        public void SaveProjectAs(GameSetupModel gsObject, PrizeLevels.PrizeLevels plsObject, DivisionsModel divisionsList)
        {
            Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
            if (String.IsNullOrEmpty(_projectFileName))
            {
                dialog.FileName = "BoardGameGeneratorProject" + _DEFAULT_EXT;
            }
            else
            {
                dialog.FileName = _projectFileName;
            }
            dialog.DefaultExt = _DEFAULT_EXT;
            dialog.Filter = _FILTER;
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                _projectFileName = dialog.FileName;
                isProjectSaved = true;
                SaveProject(gsObject, plsObject, divisionsList);
            }
        }

        public bool OpenProject()
        {
            bool loadSuccessful = true;
            Microsoft.Win32.OpenFileDialog openDialog = new Microsoft.Win32.OpenFileDialog();
            openDialog.DefaultExt = _DEFAULT_EXT;
            openDialog.Filter = _FILTER;
            bool? result = openDialog.ShowDialog();
            bool isCorrectFileType = System.Text.RegularExpressions.Regex.IsMatch(openDialog.FileName, _DEFAULT_EXT);

            if (result == true && isCorrectFileType) //User pressed OK and the extension is correct
            {
                loadSuccessful = true;
                _projectFileName = openDialog.FileName;

                IFormatter format = new BinaryFormatter();
                Stream stream = new FileStream(_projectFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                ProjectData loadedProject = (ProjectData)format.Deserialize(stream);
                SavedPrizeLevels = loadedProject.SavedPrizeLevels;
                SavedGameSetup = loadedProject.SavedGameSetup;
                SavedDivisions = loadedProject.SavedDivisions;
            }
            else if (result == true && !isCorrectFileType) //User pressed OK, but the extension is incorrect
            {
                System.Windows.MessageBox.Show("The file must be of type " + _DEFAULT_EXT);
                loadSuccessful = this.OpenProject();
            }
            else if (result == false) //User pressed Cancel or closed the dialog box
            {
                loadSuccessful = false;
            }

            return loadSuccessful;
        }
    }
}
