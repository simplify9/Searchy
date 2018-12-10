using System;

namespace SW.Searchy
{
    [Serializable()]
    public class OrderByOptions
    {
        public enum Order
        {
            ASC = 1,
            DEC = 2
        }


        private string _MemberName;

        public string MemberName
        {
            get
            {
                return _MemberName;
            }
            set
            {
                _MemberName = value;
            }
        }


        // Private _MemberType As System.Type
        // Public Property MemberType() As Type
        // Get
        // Return _MemberType
        // End Get
        // Set(ByVal value As Type)
        // _MemberType = value
        // End Set
        // End Property


        private Order _SortOrder;

        public Order SortOrder
        {
            get
            {
                return _SortOrder;
            }
            set
            {
                _SortOrder = value;
            }
        }

        public OrderByOptions(string MemberName, Order SortOrder)
        {
            _MemberName = MemberName;
            // _MemberType = MemberType
            _SortOrder = SortOrder;
        }

        public OrderByOptions()
        {
        }
    }
}
