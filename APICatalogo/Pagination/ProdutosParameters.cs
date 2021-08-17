﻿namespace APICatalogo.Pagination
{
    public class ProdutosParameters
    {
        const int maxPageSize = 50;
        private int _pageSize { get; set; } = 10;

        public int PageNumber { get; set; } = 1;
        
        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = (value > maxPageSize) ? maxPageSize : value;
            }
        }
    }
}
