using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Threading;
using System.Collections.ObjectModel;
using System.Windows.Documents;
using System.Windows.Input;
using ChupaChupa_Model.Entities;


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



        public ICommand LoadCategoryCmd  { get; set; }
        public ICommand LoadAllCategoriesCmd { get; set; }

        public ICommand DeleteCategoryCmd { get; set; }
        public ICommand AddCategoryCmd { get; set; }
        public ICommand AddCategoryCmdValidate { get; set; }
        public ICommand EditCategoryCmd  { get; set; }
        public ICommand EditCategoryCmdValidate { get; set; }

        
    }
}
