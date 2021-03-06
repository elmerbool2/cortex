﻿using CQRSlite.Commands;
using Nancy;
using neurUL.Cortex.Application.Neurons.Commands;
using System;

namespace neurUL.Cortex.Port.Adapter.In.Api
{
    public class NeuronModule : NancyModule
    {
        public NeuronModule(ICommandSender commandSender) : base("/cortex/neurons")
        {
            this.Post(string.Empty, async (parameters) =>
            {
                return await Helper.ProcessCommandResponse(
                        commandSender,
                        this.Request,
                        false,
                        (bodyAsObject, bodyAsDictionary, expectedVersion) =>
                        {
                            return new CreateNeuron(
                                Guid.Parse(bodyAsObject.Id.ToString()),
                                Guid.Parse(bodyAsObject.AuthorId.ToString())
                                );                            
                        },
                        "Id",
                        "AuthorId"
                    );
            }
            );

            this.Delete("/{neuronId}", async (parameters) =>
            {
                return await Helper.ProcessCommandResponse(
                        commandSender,
                        this.Request,
                        (bodyAsObject, bodyAsDictionary, expectedVersion) =>
                        {
                            return new DeactivateNeuron(
                                Guid.Parse(parameters.neuronId),
                                Guid.Parse(bodyAsObject.AuthorId.ToString()),
                                expectedVersion
                                );
                        },
                        "AuthorId"
                    );
            }
            );
        }
    }
}
