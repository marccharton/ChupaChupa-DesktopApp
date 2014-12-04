using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Policy;
using System.Windows;
using System.Threading;
using System.Collections.ObjectModel;
using System.Windows.Documents;
using System.Windows.Input;
using Chupachupa_DesktopApp.PrivateService;
using Chupachupa_DesktopApp.Tools;


namespace Chupachupa_DesktopApp.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// </summary>
    public partial class ViewModelDataSource : INotifyPropertyChanged
    {
        private IList<Category> _categoryList;
        public IList<Category> CategoryList
        {
            get { return _categoryList; }
            set
            {
                _categoryList = value;
                NotifyPropertyChanged("CategoryList");
            }
        }

        private Category _selectedCategory;
        public Category SelectedCategory
        {
            get { return _selectedCategory; }
            set
            {
                _selectedCategory = value;
                NotifyPropertyChanged("SelectedCategory");
            }
        }

        private Category _currentCategory;
        public Category CurrentCategory
        {
            get { return _currentCategory; }
            set
            {
                _currentCategory = value;
                NotifyPropertyChanged("CurrentCategory");
            }
        }

        private Category _newCategoryForChannel;
        public Category NewCategoryForChannel
        {
            get { return _newCategoryForChannel; }
            set
            {
                _newCategoryForChannel = value;
                NotifyPropertyChanged("NewCategoryForChannel");
            }
        }

        private bool _isFlyoutAddCategoryOpenned;
        public bool IsFlyoutAddCategoryOpenned
        {
            get { return _isFlyoutAddCategoryOpenned; }
            set
            {
                _isFlyoutAddCategoryOpenned = value;
                NotifyPropertyChanged("IsFlyoutAddCategoryOpenned");
            }
        }

        private bool _isFlyoutEditCategoryOpenned;
        public bool IsFlyoutEditCategoryOpenned
        {
            get { return _isFlyoutEditCategoryOpenned; }
            set
            {
                _isFlyoutEditCategoryOpenned = value;
                NotifyPropertyChanged("IsFlyoutEditCategoryOpenned");
            }
        }

        private string _flyoutMessage;
        public string FlyoutMessage
        {
            get { return _flyoutMessage; }
            set
            {
                _flyoutMessage = value;
                NotifyPropertyChanged("FlyoutMessage");
            }
        }

        private string _oldNameCategory;

        public ICommand LoadCategoryCmd  { get; set; }
        public ICommand LoadAllCategoriesCmd { get; set; }

        public ICommand DeleteCategoryCmd { get; set; }
        public ICommand AddCategoryCmd { get; set; }
        public ICommand AddCategoryCmdValidate { get; set; }
        public ICommand EditCategoryCmd  { get; set; }
        public ICommand EditCategoryCmdValidate { get; set; }


        private void LoadCategories()
        {
            try
            {
                IList<Category> ret = _serveur.getCategories();
                if (_serveur != null && ret != null)
                    CategoryList = ret.ToList(); 
            }
            catch (Exception e)
            {
                if (e.InnerException != null)
                    DebugText = e.InnerException.Message;
                else
                    DebugText = e.Message;
            }
        }

        private void ManageCategories()
        {
            // ******** suppression d'une catégorie *********
            DeleteCategoryCmd = new Command(new Action(() =>
            {
                try
                {
                    if (SelectedCategory != null)
                    {
                        CategoryList.Remove(SelectedCategory);
                        IList<Category> correctList = CategoryList;

                        _serveur.dropCategoryWithId(SelectedCategory.IdEntity);
                        //_serveur.dropCategoryWithCategoryName(SelectedCategory.Name);
                        CategoryList = null;
                        CategoryList = correctList;
                        SelectedCategory = null;
                    }
                }
                catch (Exception e)
                {
                    if (e.InnerException != null)
                        DebugText = e.InnerException.Message;
                    else
                        DebugText = e.Message;
                }


                
            }));


            // ******** ajout d'une catégorie *********
            AddCategoryCmd = new Command(new Action(async () =>
            {
                IsFlyoutAddCategoryOpenned = true;
                FlyoutMessage = "";
                //IsTabControlEnabled = false;
                CurrentCategory = new Category();
            }));

            AddCategoryCmdValidate = new Command(new Action(async () =>
            {
                try
                {
                    CurrentCategory = _serveur.addCategory(CurrentCategory.Name);

                    //CategoryList.Add(CurrentCategory);

                    //IList<Category> correctList = CategoryList;
                    //CategoryList = null;
                    //CategoryList = correctList;

                    CategoryList = LocalData.RefreshCategories(_serveur);

                    IsFlyoutAddCategoryOpenned = false;
                    SelectedTabIndex = (int)ToolsBox.TabIndex.TAB_CATEGORY;
                    FlyoutMessage = "";
                }
                catch (Exception e)
                {
                    if (e.InnerException != null)
                        FlyoutMessage = e.InnerException.Message;
                    else
                        FlyoutMessage = e.Message;
                }
            }));


            // ******** edition d'une catégorie *********
            EditCategoryCmd = new Command(new Action(async () =>
            {
                if (SelectedCategory != null)
                {
                    CurrentCategory = SelectedCategory;
                    IsFlyoutEditCategoryOpenned = true;
                    FlyoutMessage = "";
                    _oldNameCategory = CurrentCategory.Name;
                    //IsTabControlEnabled = false;
                }
            }));

            EditCategoryCmdValidate = new Command(new Action(async () =>
            {
                try
                {
                    _serveur.renameCategory(CurrentCategory.IdEntity, CurrentCategory.Name);
                    IsFlyoutEditCategoryOpenned = false;
                    CategoryList = LocalData.RefreshCategories(_serveur);
                }
                catch (Exception e)
                {
                    if (e.InnerException != null)
                        FlyoutMessage = e.InnerException.Message;
                    else
                        FlyoutMessage = e.Message;

                    if (CurrentCategory != null) {
                        CurrentCategory.Name = _oldNameCategory;
                    }
                }
            }));
        }
    }
}
