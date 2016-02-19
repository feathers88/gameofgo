using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using GoG.Database;
using GoG.Infrastructure.Services.Engine;
using GoG.ServerCommon.Logging;

namespace GoG.Repository.Log
{
    public class DbLogRepository : ILogRepository
    {
        public void Log(Guid gameId, LogLevel level, object ctx, Exception ex)
        {
            try
            {
                using (var dbctx = new GoGEntities())
                {
                    var serializedCtx = ctx == null ? null : Serialize(ctx);
                    var exStr = ex == null ? null : ex.ToString();
                    // Trim to db constraint.
                    string exTypeStr = null;
                    if (ex != null)
                    {
                        if (ex is GoEngineException)
                            exTypeStr = (ex as GoEngineException).Code.ToString();
                        else
                        {
                            var t = ex.GetType().Name;
                            // Database takes only 50 characters.
                            exTypeStr = t.Length > 50 ? t.Substring(0, 50) : t;
                        }
                    }
                    dbctx.Logs.Add(new Database.Log()
                    {
                        GameId = gameId == Guid.Empty ? (Guid?)null : gameId,
                        Date = DateTimeOffset.Now,
                        ExceptionType = exTypeStr,
                        Level = level.ToString(),
                        Context = serializedCtx,
                        Exception = exStr
                    });
                    dbctx.SaveChanges();
                }
            }
            catch (Exception)
            {
                // eat it
            }
        }

        // Cache the serializers for speed.
        private readonly Dictionary<Type, XmlSerializer> _serializers = new Dictionary<Type, XmlSerializer>();

        private string Serialize(object o)
        {
            if (o == null) throw new ArgumentNullException("o");

            if (o is string)
                return (string)o;
            if (o is Guid)
                return o.ToString();

            XmlSerializer serializer;
            //lock (_serializers)
            //{
                var type = o.GetType();
                if (_serializers.ContainsKey(type))
                    serializer = _serializers[type];
                else
                {
                    serializer = new XmlSerializer(o.GetType());
                    _serializers.Add(type, serializer);
                }
            //}
            var sw = new StringWriter();
            serializer.Serialize(sw, o);
            var rval = sw.ToString();

            return rval;
        }
    }
}
