# Nop.Plugin.Misc.Syslog
Send shortmessage and fullmessage (separated) to syslog server by schedule task.
Clear logs from database.

Can truncate fullmessage by settings => syslogsettings.truncationtextjsonarray=["--- End of inner exception stack trace ---","--- End of stack trace from previous location ---"]

All customized settings:
syslogsettings.appname	nop_shop		
syslogsettings.batchsize	500	
syslogsettings.currentlogid	0	
syslogsettings.facility	16	
syslogsettings.host	10.30.14.12	
syslogsettings.hostname	web-server	
syslogsettings.levels	30,40,50		
syslogsettings.limitamount	500000	
syslogsettings.limitdays	90		
syslogsettings.port	514
syslogsettings.servertimezone	Eastern Standard Time
syslogsettings.truncationtextjsonarray ["--- End of inner exception stack trace ---","--- End of stack trace from previous location ---"]
