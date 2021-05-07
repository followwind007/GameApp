"""Usage:
  FontPruner.py --inputPath=<inputPaths>... --inputFont=<inputFont>...  [--tempPath=<tempPath>]
"""
from docopt import docopt
import os

SepPath = os.path.sep
TempPathDefault = "tmp"
InputFilelist = "input_filelist.txt"
IntermediateFolder = "intermediate"
OutputFolder = "output"
ChineseOutPut = "ChineseOutPut.txt"
UnChineseOutPut = "unChineseOutPut.txt"
Succ = 0
def genFilePathList(inputPath,FileListOP):    
  fullPara = ""
  for path in inputPath:
    print "path = " +path
    fullPara +=path+" "
  fullPara += " "+FileListOP + SepPath+InputFilelist    
  command ="java -jar bin"+SepPath+"GenFileList.jar " +fullPara
  if os.system(command) is not Succ:
    raise Exception('generate fileList.txt error!'+command)
  print("genFilePathList completed" +"   " +fullPara)


def extractFileString(temp):
  fileListPath = temp+SepPath+InputFilelist
  outputPath = temp+SepPath+IntermediateFolder

  command ="java -jar bin"+SepPath+"fontExtract.jar " +fileListPath + " "+outputPath
  if os.system(command) is not Succ:
    raise Exception('extract font string  error!'+command)
  print("extractFileString completed")


def bulidNewFont(originPath,outPutPath):
    fullOutPut = outPutPath+SepPath+OutputFolder
    if not os.path.exists(fullOutPut):
      os.makedirs(fullOutPut)
    for fontOrigin in originPath:
      index = len(fontOrigin) - fontOrigin.rfind(SepPath)
      fontName = fontOrigin[-index:]
      print ("fontName" +fontName)
      fullPara = ""
      fullPara += outPutPath+SepPath+IntermediateFolder+SepPath+ChineseOutPut+"  "+outPutPath+SepPath+IntermediateFolder+SepPath+UnChineseOutPut+" "+fontOrigin + " " + fullOutPut+SepPath+fontName 
      command ="java -jar bin"+SepPath+"sfnttool.jar -c " +fullPara
      
      if os.system(command) is not Succ:
        raise Exception('build new font error!'+command)
      print("bulidNewFont completed"+ fontOrigin)


if __name__ == '__main__':
  arguments = docopt(__doc__, version='0.1.1rc')
  print(arguments)
        #raise Exception("path not exists")

  for path in arguments['--inputPath']:
    if not os.path.exists(path):
      raise Exception("inputPath - bad path: " + path)

  for path in arguments['--inputFont']:
      if not os.path.exists(path):
         raise Exception("inputFont - bad path: " + path)

  tmp = arguments['--tempPath']
  if tmp is None:
    tmp = TempPathDefault
  else :
      if not os.path.exists(tmp):
        raise Exception("tempPath - bad path: " + tmp) 

  genFilePathList(arguments['--inputPath'], tmp) 

  extractFileString(tmp)
  
  bulidNewFont(arguments['--inputFont'], tmp)

  
           
    
