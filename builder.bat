ECHO Creating Directory For Output
MKDIR HTML_PROJECT
DIR HTML_PROJECT
ECHO Building project 
csc /out:test.exe Post_Processor_System\*.cs Parser\*.cs HTML_Builder_Systeem\*.cs File_Ingest_System\*.cs Display\*.cs CSharp_Analyzer_Core\*.cs