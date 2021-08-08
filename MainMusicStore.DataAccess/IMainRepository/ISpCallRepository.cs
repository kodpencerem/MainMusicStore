using Dapper;
using System;
using System.Collections.Generic;

namespace MainMusicStore.DataAccess.IMainRepository
{
    //Dapper, Stackoverflow tarafından geliştirilen ve birçok veri tabanını destekleyen ORM aracıdır. Performanslı bir ORM aracı olduğu için genellikle büyük yapılarda kullanılmaktadır. 

    /// <summary>
    /// IDisposible garbage collector çağırır. 
    /// </summary>
    public interface ISpCallRepository : IDisposable
    {
        T Single<T>(string procedureName, DynamicParameters dynamicParameters = null);

        void Execute(string procedureName, DynamicParameters dynamicParameters = null);

        T OneRecord<T>(string procedureName, DynamicParameters dynamicParameters = null);

        IEnumerable<T> List<T>(string procedureName, DynamicParameters dynamicParameters = null);

        Tuple<IEnumerable<T1>, IEnumerable<T2>> List<T1, T2>(string procedureName, DynamicParameters dynamicParameters = null);
    }
}
