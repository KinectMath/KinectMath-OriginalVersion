using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace MDI_PlotGraph_Integration.Model.DataType
{
    class TableItem : INotifyPropertyChanged
    {
        public TableItem() { }

        private float _X = 0f;
        public float Table_X
        {
            get { return _X; }
            set
            {
                if (_X != value)
                {
                    float oldValue = _X;
                    _X = value;
                    XChanged(oldValue, value);
                    RaisePropertyChanged("X");
                }
            }
        }

        protected virtual void XChanged(float oldValue, float newValue)
        {
        }


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
