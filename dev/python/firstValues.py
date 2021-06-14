import pathlib
import pyabf

if __name__ == "__main__":
    abfFolder = pathlib.Path(__file__).parent.parent.joinpath("abfs")
    for abfFilePath in abfFolder.glob("*.abf"):
        abf = pyabf.ABF(str(abfFilePath))
        firstValues = [str(x) for x in abf.sweepY[:5]]
        print(abfFilePath.name, ", ".join(firstValues))