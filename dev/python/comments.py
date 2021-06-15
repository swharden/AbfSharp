import pathlib
import pyabf

if __name__ == "__main__":
    abfFolder = pathlib.Path(__file__).parent.parent.joinpath("abfs")
    for abfFilePath in abfFolder.glob("*.abf"):
        abf = pyabf.ABF(str(abfFilePath))
        if not abf.tagComments:
            print(abfFilePath.name, "NONE")
            continue
        else:
            print(abfFilePath.name, abf.tagComments, abf.tagTimesSec)