// See https://aka.ms/new-console-template for more information

namespace EasyScript
{
    /// <summary>
    /// 引数のデータ
    /// </summary>
    /// <param name="name">引数の名前</param>
    /// <param name="default">引数のデフォルト、無ければ必須</param>
    public record struct ArgData(string name, object? @default=null);
}