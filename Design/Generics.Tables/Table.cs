using System;
using System.Collections.Generic;

namespace Generics.Tables
{
    public class Table<TRow, TColumn, TValue>
    {
        private Dictionary<RowColumn, TValue> _data;

        public HashSet<TRow> Rows { get; }
        public HashSet<TColumn> Columns { get; }
        public Table<TRow, TColumn, TValue> Open { get => this; }
        public TableExisted<TRow, TColumn, TValue> Existed { get; }

        public Table()
        {
            _data = new Dictionary<RowColumn, TValue>();

            Rows = new HashSet<TRow>();
            Columns = new HashSet<TColumn>();
            Existed = new TableExisted<TRow, TColumn, TValue>(this);
        }

        public void AddRow(TRow row)
        {
            Rows.Add(row);
        }

        public void AddColumn(TColumn column)
        {
            Columns.Add(column);
        }

        public TValue this[TRow row, TColumn column]
        {
            get
            {
                return _data.TryGetValue(new RowColumn(row, column), out TValue value)
                    ? value
                    : default;
            }
            set
            {
                if (!Rows.Contains(row))
                    Rows.Add(row);
                if (!Columns.Contains(column))
                    Columns.Add(column);
                _data[new RowColumn(row, column)] = value;
            }
        }

        private struct RowColumn
        {
            public TRow Row;
            public TColumn Column;

            internal RowColumn(TRow row, TColumn column)
            {
                Row = row;
                Column = column;
            }
        }
    }

    public class TableExisted<TRow, TColumn, TValue>
    {
        private readonly Table<TRow, TColumn, TValue> _table;

        public TableExisted(Table<TRow, TColumn, TValue> table)
        {
            _table = table;
        }

        public TValue this[TRow row, TColumn column]
        {
            get
            {
                if (!_table.Rows.Contains(row))
                    throw new ArgumentException($"Row does not existed: {nameof(row)}");
                if (!_table.Columns.Contains(column))
                    throw new ArgumentException($"Column does not existed: {nameof(column)}");
                return _table[row, column];
            }
            set
            {
                if (!_table.Rows.Contains(row))
                    throw new ArgumentException($"Row does not existed: {nameof(row)}");
                if (!_table.Columns.Contains(column))
                    throw new ArgumentException($"Column does not existed: {nameof(column)}");
                _table[row, column] = value;
            }
        }
    }
}