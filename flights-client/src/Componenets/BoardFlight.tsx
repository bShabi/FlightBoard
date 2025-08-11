import {  useState } from "react";
import CustomTable from "./CustomTable";
import AddFlightForm from "./AddFlightForm";
import { useFlight } from "../UseHook/useFlight";
import { Grid } from "@mui/material";
import SearchFlight from "./SearchFlight";
import { Flight } from "../Types/FlightTypes";
const BoardFlight = () => {
  const columns = [
    { id: "flightNumber", label: "Flight Number" },
    { id: "destination", label: "Destination" },
    { id: "departureTime", label: "Departure Time" },
    { id: "gate", label: "Gate" },
    { id: "status", label: "Status" },
  ];

  const [sortBy, setSortBy] = useState("flightNumber");
  const [isSearch, setIsSearch] = useState(false);
  const {
    selectedFlight,
    flights,
    filteredFlights,
    error,
    loading,
    lastUpdatedId,
    clearLastUpdated,
    deleteFlight,
    setFlight,
    clearSelectedFlight,
    clearFilteredFlight,
    setLoadingToUpdate,
    querySearchFlight,
    setMessageError
  } = useFlight();

  const handleDelete = (guid: string) => {
    setLoadingToUpdate();
    deleteFlight(guid);
  };
  const handleValidation = (candidate: Flight): string | null => {
    if (!candidate.flightNumber?.trim()) return "Flight number is required";
    if (!candidate.destination?.trim()) return "Destination is required";
    if (!candidate.gate?.trim()) return "Gate is required";
    if (!candidate.departureTime?.trim()) return "Departure time is required";

    const findFlight = flights.some(
      (f) => f.flightNumber === candidate.flightNumber
    );
    if (findFlight) return "Flight number already exists";

    // Validate time
    const dep = new Date(candidate.departureTime);
    if (Number.isNaN(dep.getTime())) return "Departure time is invalid";
    return null;
  };

  return (
    <div className="App" style={{ padding: "2rem" }}>
      <h2>Board Flights</h2>
      <Grid container spacing={2} sx={{ justifyContent: "center" }}>
        <Grid component="div">
          <AddFlightForm
            selectedFlight={selectedFlight}
            clearSelection={() => clearSelectedFlight()}
            setLoadingToUpdate={() => setLoadingToUpdate()}
            handleValidation={handleValidation}
          />
        </Grid>

        <Grid component="div">
          <SearchFlight
            onSearch={(query) => {
              setIsSearch(true);
               setMessageError(null)
              if(query==="ALL_DATA") {
                 setIsSearch(false)
                 
                 return
              }
              querySearchFlight(query);
            }}
            clearFilteredFlight={() => {
              clearFilteredFlight();
              setIsSearch(false);
            }}
          />
        </Grid>
      </Grid>
      {error && <p style={{ color: "red" }}>{error}</p>}
      {isSearch && filteredFlights && filteredFlights.length === 0 ? (
        <p style={{ color: "red" }}>No flights found</p>
      ) : (
        <CustomTable
          data={
            filteredFlights && filteredFlights?.length > 0
              ? filteredFlights
              : flights || []
          }
          isLoading={loading}
          columns={columns}
          filterColumn={sortBy}
          onHeaderClick={(col) => setSortBy(col)}
          onHandleClick={(flight) => {
            console.log("צפייה בטיסה:", flight);
            setFlight(flight.guid);
          }}
          onHandleRemoveClick={(flight) => handleDelete(flight.flightNumber)}
          highlightedId={lastUpdatedId}
          onHighlightConsumed={() => {
            clearLastUpdated();
          }}
        />
      )}
    </div>
  );
};
export default BoardFlight;
