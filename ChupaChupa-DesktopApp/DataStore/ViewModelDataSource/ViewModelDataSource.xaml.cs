//      *********    NE PAS MODIFIER CE FICHIER     *********
//      Ce fichier est régénéré par un outil de création.Modifier
// .     ce fichier peut provoquer des erreurs.
namespace Expression.Blend.DataStore.ViewModelDataSource
{
	using System;
	using System.Collections.Generic;

	public class ViewModelDataSourceGlobalStorage
	{
		public static ViewModelDataSourceGlobalStorage Singleton;
		public bool Loading {get;set;}
		private List<WeakReference> registrar; 

		public ViewModelDataSourceGlobalStorage()
		{
			this.registrar = new List<WeakReference>();
		}
		
		static ViewModelDataSourceGlobalStorage()
		{
			Singleton = new ViewModelDataSourceGlobalStorage();
		}

		public void Register(ViewModelDataSource dataStore)
		{
			this.registrar.Add(new WeakReference(dataStore));
		}

		public void OnPropertyChanged(string property)
		{
			foreach (WeakReference entry in this.registrar)
			{
				if (!entry.IsAlive)
				{
					continue;
				}
				ViewModelDataSource dataStore = (ViewModelDataSource)entry.Target;
				dataStore.FirePropertyChanged(property);
			}
		}
		
		public bool AssignementAllowed
		{
			get
			{
				// Attribution unique après chargement
				if(this.Loading && this.registrar.Count > 0)
				{
					return false;
				}
				
				return true;
			}
		}

		private string _Property1 = string.Empty;

		public string Property1
		{
			get
			{
				return this._Property1;
			}

			set
			{
				if(!this.AssignementAllowed)
				{
					return;
				}
				
				if (this._Property1 != value)
				{
					this._Property1 = value;
					this.OnPropertyChanged("Property1");
				}
			}
		}
	}

	public class ViewModelDataSource : System.ComponentModel.INotifyPropertyChanged
	{
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		
		public void FirePropertyChanged(string propertyName)
		{
			this.OnPropertyChanged(propertyName);
		}

		protected virtual void OnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}

		public ViewModelDataSource()
		{
			try
			{
				System.Uri resourceUri = new System.Uri("/Chupachupa-DesktopApp;component/DataStore/ViewModelDataSource/ViewModelDataSource.xaml", System.UriKind.Relative);
				if (System.Windows.Application.GetResourceStream(resourceUri) != null)
				{
					ViewModelDataSourceGlobalStorage.Singleton.Loading = true;
					System.Windows.Application.LoadComponent(this, resourceUri);
					ViewModelDataSourceGlobalStorage.Singleton.Loading = false;
					ViewModelDataSourceGlobalStorage.Singleton.Register(this);
				}
			}
			catch (System.Exception)
			{
			}
		}

		private string _Property1 = string.Empty;

		public string Property1
		{
			get
			{
				return ViewModelDataSourceGlobalStorage.Singleton.Property1;
			}

			set
			{
				ViewModelDataSourceGlobalStorage.Singleton.Property1 = value;
			}
		}
	}
}
