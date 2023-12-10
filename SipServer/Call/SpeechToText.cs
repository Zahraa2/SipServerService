﻿using Google.Api.Gax.Grpc;
using Google.Cloud.Speech.V1;
using Microsoft.Extensions.Configuration;
using SIPServer.Models;
using System;
using System.Threading.Tasks;
using static Google.Cloud.Speech.V1.SpeechClient;
using static System.Net.Mime.MediaTypeNames;

namespace SIPServer.Call
{
    class SpeechToText : KHService
    {
        private SpeechClient                _client;
        private StreamingRecognizeStream    _streamingCall;
        public readonly int                 SAMPLE_RATE;

        public SpeechToText( SIPCall call) : base(call)
        {
                SAMPLE_RATE = 8000;
        }

        public override async Task Initialization()

        {
            _client = SpeechClient.Create();
            _call.Log("Create the Speech client");
            _streamingCall = _client.StreamingRecognize();
            // The response stream
            var responseStream = _streamingCall.GetResponseStream();

            var streamingConfig = new StreamingRecognitionConfig
            {
                Config = new RecognitionConfig
                {
                    Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
                    SampleRateHertz = SAMPLE_RATE,
                    LanguageCode = LanguageCodes.Arabic.Egypt,
                },
                InterimResults = false,
            };

            _call.Log("Streaming Encoding Codecs => Linear16");
            _call.Log($"Streaming Sample Rate => ${SAMPLE_RATE}");
            _call.Log($"Streaming LanguageCode => ${LanguageCodes.Arabic.Egypt}");

            await _streamingCall.WriteAsync(new StreamingRecognizeRequest
            {
                StreamingConfig = streamingConfig
            });

            // Start tasks
            _ = Transcript(responseStream);
        }

        private async Task Transcript(AsyncResponseStream<StreamingRecognizeResponse> responseStream)
        {
            string text;

            try
            {
                await foreach (var response in responseStream)
                {
                    foreach (var result in response.Results)
                    {
                        if (!result.IsFinal)
                            continue;

                        text = result.Alternatives[0].Transcript;


                        if (!_call.IsRunning && !string.IsNullOrEmpty(text))
                        {
                            _call.Log($"Transcript Added: {text}");

                            _call.TranscriptedText.Add(text);
                            _call.IsRunning = true;
                        }
                        else
                            _call.Log($"Transcript Not Added: {text}");

                    }
                }
            }
            catch (Exception e)
            {
                _call.Log($"Exception: {e}");
            }
        }

        public override void main()
        {
            try
            {
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    byte[] buffer = _call.CallAudio.Take(); 

                    if (buffer.Length > 0)
                    {
                        _streamingCall.WriteAsync(new StreamingRecognizeRequest
                        {
                            AudioContent = Google.Protobuf.ByteString.CopyFrom(buffer)
                        });
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _call.Log("Streaming was canceled.");
            }
            finally
            {
                _call.Log("Streaming Completed");
                _streamingCall.WriteCompleteAsync();

            }
        }

    }
}
