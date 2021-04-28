REM This should only be made once!
nats stream rm yggiomeasurementbridge --force
nats stream create natssamplestream  --subjects=natssamplestream.samplesubject.timestamp.received.*  --ack --max-msgs=-1 --max-age=-1 --max-bytes=-1 --max-msg-size=-1 --storage=file --retention=limits --discard=old
