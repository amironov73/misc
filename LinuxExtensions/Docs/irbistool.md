# irbistool

Общий формат командной строки

```bash
irbistool tool name, parameters
```

available tools:

* actualif - actualize database dictionary
* batch - batch command execution
* cells - build facet cache
* cleanmtx - clean record mutexes
* delcells - clear facet cache
* diaglnk - diagnose dictionary
* diagmfn - diagnose mst and xrf
* diagmstxrf - diagnose mst and xrf coincidence
* diagrec - diagnose records
* diagrecft - diagnose full text records
* dict - create database dictionary
* dropdb - drop database
* emptydb - empty database fully
* emptyftdb - empty fulltext database only
* export - export records from database
* exportif - export inverted file
* fmtprint - convert format file to minimized single line or pretty look with indentation
* format - compile and run formats for error checking
* ftcache - build fulltext cache
* ftinput - input full texts
* global - global correction
* ifs2fs - ifs file convertion to classic fst format
* import - import records to database
* loaddict - load database dictionary only
* lockdb - lock database
* mergedb - merge multiple databases into one without indexing
* newdb - create new database
* pdfclean - clean pdf files
* reorgexmf - reorganize master file excluding deleted records (export / import)
* reorgif - reorganize inverted file
* reorgmf - reorganize master file
* sort - sort terms only
* terms - record terms only
* unlockdb - unlock database
* unlockmfn - unlock records by mfn range or list
* webadddb - make database available for web
* webdeldb - make database unavailable for web

## diagmfn

```
irbistool diagmfn -i "ini file path" -d "database name" [-mf "mfn from, default 1" -mt "mfn to default last db mfn"]
```

Example: 

```
irbistool diagmfn -i irbisa.ini -d dbtest -mf 1 -mt 10
```

## format

```
irbistool format -i "ini file path" -d "database name" -src "directory with formats or file mask" [-rpt "generate report in csv format" -rptjson "generate report in json format" -r mfn range from-to (or -mf mfn from, default 1 and -mt mfn to default last db mfn) or -l mfn list from file or -se "search expr" or -sc "sequential search expr"]
```

Example: 

```
irbistool format -i irbisa.ini -d ibis -src format_test
```

Example: 

```
irbistool format -i irbisa.ini -d ibis -src format_test -mf 1 -mt 10
```

Example: 

```
irbistool format -i irbisa.ini -d * -rpt -src *.pft -mf 1 -mt 10
```


## newdb

```
irbistool newdb -i "ini file path" -d "database name" [-f "database full name" -t template database name -fr free db (no files from IBIS) -r for reader -w for reader web]
```

Example: 

```
irbistool newdb -i irbisa.ini -d dbtest -f "test database" -r -w
```

