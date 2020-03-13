# ExaScale
A data sharding management

![.NET Core Build](https://github.com/mafshin/exascale/workflows/.NET%20Core%20Build/badge.svg)
![.NET Core Test](https://github.com/mafshin/exascale/workflows/.NET%20Core%20Test/badge.svg)

Managing large volumes of enterprise data usually will need rearchitecting the way data is stored in databases. One of well-known techniques is distributing data across multiple shards to get better performance during data read and write.

**ExaScale** tries to bring a provider agnostic solution to the table. Each provider must impelment its own operations to be supported.

## Performance Test (SQL Provider)

| Shards Count | Total Shard Items | Records Per Shard Item | Time (sec)
---------------|:------------------|:-----------------------|:-----------|
| 1 | 100,000 | 10 | 44
| 5 | 100,000 | 10 | 36

