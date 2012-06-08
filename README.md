TSDumper
========

Scheduled advanced DVB Stream dumper application for DVB-S cards on Windows.

<a href="https://github.com/esurharun/TSDumper/blob/master/doc/screenshot.jpg?raw=true">
<img src="https://github.com/esurharun/TSDumper/blob/master/doc/screenshot.jpg?raw=true" />
</a>

Mantis driver on Ubuntu lacks with some tuning problems  but windows driver for Technisat SkyStar HD2. 
So i started to move my project DVBRec2 to windows which i wrote with Scala but i couldn't find a good 
way to dump TS streams to disk for further processes. 

DVPiper was working good but is not open-source and other DVB applications are more complicated and lacks
with customizing. Mediaportal was ok but Twinhan tuner support was not that good with TVEngine3. 

At last, i found EPG Centre which is still under development by Steve Bickell and modified it with my own
purposes.

Only thing that you may not understand is the ***scheduling mechanism***.

Let me explain. I want to record TS streams on time bases sharp. For example my normal recordings are like
02:00-04:00 04:00-06:00 ... etc period. But when i start to dump time cannot be at 02:00 or 04:00 sharp etc. 

So, i added ***start hour*** field to give timing basis. So if start at 02:35 and start hour is 00:00 and
seconds is 7200secs (2 hours), it will restart-recording at 04:00 so recordings will be like

02:35-04:00 , 04:00-06:00 etc..

For example if you want to record 30minutes basis, you can just switch start hour to 00:30 and give 1800secs.

***TODO***
=============================

- Automatic searching frequencies and getting pids for easier selection of channels
- Pagination based multi-tuner recording interface
- More advanced view of signal rates, recording/buffer sizes with progress bars
- FFMpeg integration (maybe)



For support please contact me at esur/\./\harun at gmail/\./\com