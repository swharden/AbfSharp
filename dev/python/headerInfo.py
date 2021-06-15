import pathlib
import pyabf

if __name__ == "__main__":
    abfFolder = pathlib.Path(__file__).parent.parent.joinpath("abfs")
    for abfFilePath in abfFolder.glob("*.abf"):
        abf = pyabf.ABF(str(abfFilePath))
        #print(abf.abfID, abf.abfDateTime)
        print(f'[TestCase("{abf.abfID}.abf", "{abf.abfDateTime}")]')
