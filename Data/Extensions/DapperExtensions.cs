using System.Data;
using System.Threading.Tasks;

namespace Data.Extensions;

public static class DapperExtensions
{
    public static async Task<IEnumerable<TReturn>> QueryAsync<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>(
        this IDbConnection cnn,
        string sql,
        Func<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> map,
        object param = null,
        IDbTransaction transaction = null,
        bool buffered = true,
        string splitOn = "Id",
        int? commandTimeout = null,
        CommandType? commandType = null)
    {
        var result = await cnn.QueryAsync(sql, new[] {
            typeof(T1), typeof(T2), typeof(T3), typeof(T4),
            typeof(T5), typeof(T6), typeof(T7), typeof(T8)
        }, objects =>
        {
            return map(
                (T1)objects[0], (T2)objects[1], (T3)objects[2], (T4)objects[3],
                (T5)objects[4], (T6)objects[5], (T7)objects[6], (T8)objects[7]
            );
        }, param, transaction, buffered, splitOn, commandTimeout, commandType);

        return result;
    }

    public static async Task<IEnumerable<TReturn>> QueryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>(
        this IDbConnection cnn,
        string sql,
        Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn> map,
        object param = null,
        IDbTransaction transaction = null,
        bool buffered = true,
        string splitOn = "Id",
        int? commandTimeout = null,
        CommandType? commandType = null)
    {
        var result = await cnn.QueryAsync(sql, new[] {
            typeof(T1), typeof(T2), typeof(T3), typeof(T4),
            typeof(T5), typeof(T6), typeof(T7), typeof(T8),
            typeof(T9)
        }, objects =>
        {
            return map(
                (T1)objects[0], (T2)objects[1], (T3)objects[2], (T4)objects[3],
                (T5)objects[4], (T6)objects[5], (T7)objects[6], (T8)objects[7], (T9)objects[8]
            );
        }, param, transaction, buffered, splitOn, commandTimeout, commandType);

        return result;
    }

    public static async Task<IEnumerable<TReturn>> QueryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>(
        this IDbConnection cnn,
        string sql,
        Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn> map,
        object param = null,
        IDbTransaction transaction = null,
        bool buffered = true,
        string splitOn = "Id",
        int? commandTimeout = null,
        CommandType? commandType = null)
    {
        var result = await cnn.QueryAsync(sql, new[] {
            typeof(T1), typeof(T2), typeof(T3), typeof(T4),
            typeof(T5), typeof(T6), typeof(T7), typeof(T8),
            typeof(T9), typeof(T10)
        }, objects =>
        {
            return map(
                (T1)objects[0], (T2)objects[1], (T3)objects[2], (T4)objects[3],
                (T5)objects[4], (T6)objects[5], (T7)objects[6], (T8)objects[7],
                (T9)objects[8], (T10)objects[9]
            );
        }, param, transaction, buffered, splitOn, commandTimeout, commandType);

        return result;
    }
}