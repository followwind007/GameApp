using Tools.Table.Asset;

namespace Tools.Table.Exporter
{
    public interface IExporter
    {
        string Export(TableObject table, TableConfig config);
    }
}
