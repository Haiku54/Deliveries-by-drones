using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;


namespace PO
{
    public class DroneToList : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int _ID;
        public int ID
        {
            get { return _ID; }
            set
            {
                _ID = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ID"));
            }
        }


        private string _Model;
        public string Model
        {
            get { return _Model; }
            set
            {
                _Model = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ViewModel"));
            }
        }

        private BO.DroneStatus _Status;
        public BO.DroneStatus Status
        {
            get { return _Status; }
            set
            {
                _Status = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Status"));
            }
        }


        private double _Battery;
        public double Battery
        {
            get { return _Battery; }
            set
            {
                _Battery = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Battery"));
            }
        }


        private BO.Location _DroneLocation;
        public BO.Location DroneLocation
        {
            get { return _DroneLocation; }
            set
            {
                _DroneLocation = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("DroneLocation"));
            }
        }


        private int _PackageID;
        public int PackageID
        {
            get { return _PackageID; }
            set
            {
                _PackageID = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("PackageID"));
            }
        }

        private BO.WeightCategories _MaxWeight;
        public BO.WeightCategories MaxWeight
        {
            get { return _MaxWeight; }
            set
            {
                _MaxWeight = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("MaxWeight"));
            }
        }

        public PL.DisplayDrone DronePage { get; set; }
    }
}
