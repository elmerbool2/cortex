﻿using CQRSlite.Commands;
using neurUL.Common.Domain.Model;
using neurUL.Common.Http;
using neurUL.Cortex.Application.Neurons.Commands;
using neurUL.Cortex.Domain.Model.Neurons;
using System.Threading;
using System.Threading.Tasks;
using ei8.EventSourcing.Client;
using ei8.EventSourcing.Client.In;

namespace neurUL.Cortex.Application.Neurons
{
    public class NeuronCommandHandlers : 
        ICancellableCommandHandler<CreateNeuron>,
        ICancellableCommandHandler<DeactivateNeuron>
    {
        private readonly IEventSourceFactory eventSourceFactory;
        private readonly ISettingsService settingsService;

        public NeuronCommandHandlers(IEventSourceFactory eventSourceFactory, ISettingsService settingsService)
        {
            AssertionConcern.AssertArgumentNotNull(eventSourceFactory, nameof(eventSourceFactory));
            AssertionConcern.AssertArgumentNotNull(settingsService, nameof(settingsService));

            this.eventSourceFactory = eventSourceFactory;
            this.settingsService = settingsService;
        }

        public async Task Handle(CreateNeuron message, CancellationToken token = default(CancellationToken))
        {
            AssertionConcern.AssertArgumentNotNull(message, nameof(message));

            var eventSource = this.eventSourceFactory.Create(
                this.settingsService.EventSourcingInBaseUrl + "/",
                this.settingsService.EventSourcingOutBaseUrl + "/",
                message.AuthorId
                );

            var neuron = new Neuron(message.Id);
            
            await eventSource.Session.Add(neuron, token);
            await eventSource.Session.Commit(token);
        }

        public async Task Handle(DeactivateNeuron message, CancellationToken token = default(CancellationToken))
        {
            AssertionConcern.AssertArgumentNotNull(message, nameof(message));

            var eventSource = this.eventSourceFactory.Create(
                this.settingsService.EventSourcingInBaseUrl + "/",
                this.settingsService.EventSourcingOutBaseUrl + "/",
                message.AuthorId
                );
            Neuron neuron = await eventSource.Session.Get<Neuron>(message.Id, nameof(neuron), message.ExpectedVersion, token);

            neuron.Deactivate();
            await eventSource.Session.Commit(token);
        }
    }
}