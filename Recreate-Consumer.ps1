REM This should only be made once!
nats consumer rm natssamplestream natssampleconsumer --force
nats consumer create natssamplestream natssampleconsumer --target=natssamplestream.consumer.samplesubject.timestamp --filter="" --replay=instant --deliver=all --ack=explicit --wait=1s --max-deliver=-1
