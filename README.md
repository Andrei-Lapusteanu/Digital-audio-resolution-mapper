# Digital-audio-resolution-mapper

## Summary 

 A small test for bitrate mapping using exported 32-bit samples from Audacity, and mapping them to either 8 or 10-bits

<a href="https://imgur.com/V8MoOaD"><img src="https://i.imgur.com/V8MoOaD.png" title="source: imgur.com" /></a><br>
*App UI*

# Technologies
- Core application written in **C#**
- Autocorrelation function implemented in **C++**
- UI designed in **WPF**

# Functionalities
- Maps audio samples exported from Audacity from 32-bit to either 8 or 10-bits
- Software exists because of a project that required such data to be uploaded to an Arduino Uno board in order to test its processing capabilities

## Note
- It can currenly only process mono audio files samples
- Auto number of samples feature will be implemented in the future
<!--stackedit_data:
eyJoaXN0b3J5IjpbLTExNjk3OTk0NjMsLTE2NzY3ODU4NzAsLT
E1NzUwMzE5MDAsMTMxNTUwNTU0MSwxMjAwNDkwMTY2XX0=
-->