using System.Collections.Generic;

namespace LibOpenNFS.DataModels
{
    public class Car
    {
        public string IDOne
        {
            get
            {
                return _id1;
            }
            set
            {
                _id1 = value;
            }
        }

        public string IDTwo
        {
            get
            {
                return _id2;
            }
            set
            {
                _id2 = value;
            }
        }

        public string ModelPath
        {
            get
            {
                return _modelPath;
            }
            set
            {
                _modelPath = value;
            }
        }

        public string Maker
        {
            get
            {
                return _maker;
            }
            set
            {
                _maker = value;
            }
        }

        private string _id1;
        private string _id2;
        private string _modelPath;
        private string _maker;
    }

    public class CarList : BaseModel
    {
        public CarList(long id, long size) : base(id, size)
        {
        }

        public List<Car> Cars
        {
            get
            {
                return _cars;
            }
        }

        private List<Car> _cars = new List<Car>();
    }
}
