``` ini

BenchmarkDotNet=v0.10.9, OS=Windows 10 Redstone 2 (10.0.15063)
Processor=Intel Core i5-4670 CPU 3.40GHz (Haswell), ProcessorCount=4
Frequency=2922919 Hz, Resolution=342.1237 ns, Timer=TSC
  [Host]     : .NET Framework 4.7 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2102.0 DEBUG
  DefaultJob : .NET Framework 4.7 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2102.0


```
 | Method |      Mean |     Error |    StdDev |
 |------- |----------:|----------:|----------:|
 | Sha256 | 105.32 us | 1.3281 us | 1.2423 us |
 |    Md5 |  21.16 us | 0.1042 us | 0.0924 us |