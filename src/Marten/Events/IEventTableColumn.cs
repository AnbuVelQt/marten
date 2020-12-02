using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using LamarCodeGeneration;
using Marten.Internal.CodeGeneration;
using Marten.Linq.Parsing;
using Marten.Storage;
using Marten.Util;
using Npgsql;
using NpgsqlTypes;

namespace Marten.Events
{
    internal interface IEventTableColumn
    {
        public void GenerateSelectorCodeSync(GeneratedMethod method, EventGraph graph, int index);
        public abstract void GenerateSelectorCodeAsync(GeneratedMethod method, EventGraph graph, int index);
        public abstract void GenerateAppendCode(GeneratedMethod method, EventGraph graph, int index);
    }

    internal class EventJsonDataColumn: TableColumn, IEventTableColumn
    {
        public EventJsonDataColumn() : base("data", "jsonb", "NOT NULL")
        {
        }

        public void GenerateSelectorCodeSync(GeneratedMethod method, EventGraph graph, int index)
        {
            throw new NotImplementedException();
        }

        public void GenerateSelectorCodeAsync(GeneratedMethod method, EventGraph graph, int index)
        {
            throw new NotImplementedException();
        }

        public void GenerateAppendCode(GeneratedMethod method, EventGraph graph, int index)
        {
            throw new NotImplementedException();
        }
    }

    internal class EventTypeColumn: TableColumn, IEventTableColumn
    {
        public EventTypeColumn() : base("type", "varchar(500)", "NOT NULL")
        {
        }

        public void GenerateSelectorCodeSync(GeneratedMethod method, EventGraph graph, int index)
        {
            throw new NotImplementedException();
        }

        public void GenerateSelectorCodeAsync(GeneratedMethod method, EventGraph graph, int index)
        {
            throw new NotImplementedException();
        }

        public void GenerateAppendCode(GeneratedMethod method, EventGraph graph, int index)
        {
            throw new NotImplementedException();
        }
    }

    internal class StreamIdColumn: TableColumn, IEventTableColumn
    {
        public StreamIdColumn(EventGraph graph) : base("stream_id", "varchar")
        {
            Type = graph.GetStreamIdDBType();
            Directive = graph.TenancyStyle != TenancyStyle.Conjoined
                ? $"REFERENCES {graph.DatabaseSchemaName}.mt_streams ON DELETE CASCADE"
                : null;

        }

        public void GenerateSelectorCodeSync(GeneratedMethod method, EventGraph graph, int index)
        {
            throw new NotImplementedException();
        }

        public void GenerateSelectorCodeAsync(GeneratedMethod method, EventGraph graph, int index)
        {
            throw new NotImplementedException();
        }

        public void GenerateAppendCode(GeneratedMethod method, EventGraph graph, int index)
        {
            throw new NotImplementedException();
        }
    }

    internal class EventTableColumn: TableColumn, IEventTableColumn
    {
        private readonly Expression<Func<IEvent, object>> _eventMemberExpression;
        private readonly MemberInfo _member;

        public EventTableColumn(string name, Expression<Func<IEvent, object>> eventMemberExpression) : base(name, "varchar")
        {
            _eventMemberExpression = eventMemberExpression;
            _member = FindMembers.Determine(eventMemberExpression).Single();
            var memberType = FindMembers.Determine(eventMemberExpression).Single().GetMemberType();
            Type = TypeMappings.GetPgType(memberType, EnumStorage.AsInteger);
            NpgsqlDbType = TypeMappings.ToDbType(memberType);
        }

        public NpgsqlDbType NpgsqlDbType { get; set; }

        public void GenerateSelectorCodeSync(GeneratedMethod method, EventGraph graph, int index)
        {
            method.IfDbReaderValueIsNotNull(index, () =>
            {
                method.AssignMemberFromReader(null, index, _eventMemberExpression);
            });
        }

        public void GenerateSelectorCodeAsync(GeneratedMethod method, EventGraph graph, int index)
        {
            method.IfDbReaderValueIsNotNull(index, () =>
            {
                method.AssignMemberFromReader(null, index, _eventMemberExpression);
            });
        }

        public void GenerateAppendCode(GeneratedMethod method, EventGraph graph, int index)
        {
            method.Frames.Code($"parameters[{index}].{nameof(NpgsqlParameter.NpgsqlDbType)} = {{0}};",
                NpgsqlDbType);
            method.Frames.Code(
                $"parameters[{index}].{nameof(NpgsqlParameter.Value)} = document.{_member.Name};");
        }
    }


}
