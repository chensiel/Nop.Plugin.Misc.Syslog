# Nop.Plugin.Misc.Syslog
Send shortmessage and fullmessage (separated) to syslog server by schedule task.<br />
Clear logs from database.<br />

Can truncate fullmessage by settings => syslogsettings.truncationtextjsonarray=["--- End of inner exception stack trace ---","--- End of stack trace from previous location ---"]<br />

All customized settings example:<br />
syslogsettings.appname=nop_shop<br />	
syslogsettings.batchsize=500<br />
syslogsettings.currentlogid=0<br />
syslogsettings.facility=16<br />
syslogsettings.host=10.0.0.1<br />
syslogsettings.hostname=web-server<br />
syslogsettings.levels	30,40,50<br />
syslogsettings.limitamount=500000<br />
syslogsettings.limitdays=90<br />
syslogsettings.port=514<br />
syslogsettings.servertimezone=Eastern Standard Time<br />
syslogsettings.truncationtextjsonarray=["--- End of inner exception stack trace ---","--- End of stack trace from previous location ---"]<br />
