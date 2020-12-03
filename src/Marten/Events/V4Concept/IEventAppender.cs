using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Marten.Internal;
using Marten.Internal.Operations;
using Marten.Linq.QueryHandlers;
using Marten.Util;

namespace Marten.Events.V4Concept
{
    // This would be generated.
    public interface IEventOperationBuilder
    {
        IStorageOperation AppendEvent(EventGraph events, IMartenSession session, EventStream stream, IEvent e);
        //IStorageOperation MarkStreamVersion(EventStream stream); // Hard-coded
        IStorageOperation InsertStream(EventStream stream); // <--- This can be hard coded upfront
        IQueryHandler<StreamState> QueryForStream(EventStream stream);
    }

    public class InsertStream: IStorageOperation
    {
        private readonly EventStream _stream;

        public InsertStream(EventStream stream)
        {
            _stream = stream;
        }

        public void ConfigureCommand(CommandBuilder builder, IMartenSession session)
        {
            throw new NotImplementedException();
        }

        public Type DocumentType => typeof(EventStream);
        public void Postprocess(DbDataReader reader, IList<Exception> exceptions)
        {
            // TODO -- check that there's no existing stream?
        }

        public Task PostprocessAsync(DbDataReader reader, IList<Exception> exceptions, CancellationToken token)
        {
            // TODO -- check that there's no existing stream?
            return Task.CompletedTask;
        }

        public OperationRole Role()
        {
            return OperationRole.Events;
        }
    }

    // TODO -- also generate an ISelector<IEvent> and another for ISelector<StreamState>?
    // Nah, hard code the selector for stream state upfront


    // might need two flavors for stream guid vs string identity
    public interface IEventAppender
    {
        IEnumerable<IStorageOperation> BuildAppendOperations(IMartenSession session, IReadOnlyList<EventStream> streams);
        Task<IEnumerable<IStorageOperation>> BuildAppendOperationsAsync(IMartenSession session, IReadOnlyList<EventStream> streams, CancellationToken cancellation);

        void MarkTombstones(IReadOnlyList<EventStream> streams);
    }

    public class EventAppender: IEventAppender
    {
        private readonly EventGraph _graph;
        private readonly IEventOperationBuilder _builder;
        private readonly IInlineProjection[] _projections;

        public EventAppender(EventGraph graph, IEventOperationBuilder builder, IInlineProjection[] projections)
        {
            _graph = graph;
            _builder = builder;
            _projections = projections;

            // TODO -- split out a specific ISelector for StreamState.
            // TODO -- track the event streams separately within the DocumentSessionBase
        }

        public IEnumerable<IStorageOperation> BuildAppendOperations(IMartenSession session, IReadOnlyList<EventStream> streams)
        {
            // 1. load the stream data for each stream & reserve new sequence values.
            // 2. Assign the versions and sequence values to each event stream
            // 3. emit an operation to update each stream version
            // 4. emit an operation to insert each event
            // 5. emit operations for each inline projection
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<IStorageOperation>> BuildAppendOperationsAsync(IMartenSession session, IReadOnlyList<EventStream> streams, CancellationToken cancellation)
        {
            throw new System.NotImplementedException();
        }

        public void MarkTombstones(IReadOnlyList<EventStream> streams)
        {
            throw new System.NotImplementedException();
        }
    }
}
