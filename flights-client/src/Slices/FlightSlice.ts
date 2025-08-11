import { FlightsApiResponse } from "../Types/ResponseType";
import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import {
  getAllFlights,
  getFlightById,
  insertFlight,
  updateFlight,
  deleteFlight,
  searchByQuery,
} from "./FlightAPI";
import { Flight, FlightResponse } from "../Types/FlightTypes";
import { eErrorType } from "../Types/EnumType";
import { stat } from "fs";

interface FlightsState {
  flights: FlightResponse[];
  filteredFlights?: FlightResponse[] | null; 
  selectedFlight?: FlightResponse | null;
  loading: boolean;
  error: string | null;
  lastUpdatedId?: string | null; 
}

const initialState: FlightsState = {
  flights: [],
  filteredFlights: [],
  selectedFlight: null,
  loading: false,
  error: null,
  lastUpdatedId:null
};

// Thunks
export const fetchAllFlights = createAsyncThunk(
  "flights/fetchAll",
  async () => {
    return await getAllFlights();
  }
);

export const fetchFlightById = createAsyncThunk(
  "flights/fetchById",
  async (id: string) => {
    return await getFlightById(id);
  }
);

export const addFlight = createAsyncThunk(
  "flights/add",
  async (flight: Flight) => {
    return await insertFlight(flight);
  }
);

export const editFlight = createAsyncThunk(
  "flights/edit",
  async (flight: Flight) => {
    return await updateFlight(flight);
  }
);

export const removeFlight = createAsyncThunk(
  "flights/delete",
  async (id: string) => {
    await deleteFlight(id);
    return id;
  }
);
export const getFlightByQuery = createAsyncThunk(
  "flights/getFlightByQuery",
  async (query: string) => {
    return await searchByQuery(query);
  }
);

// Slice
const flightSlice = createSlice({
  name: "flights",
  initialState,
  reducers: {
    setlClearSelectedFlight: (state) => {
      state.selectedFlight = null;
    },
    setSelectedFlight: (state, action) => {
      state.selectedFlight = state.flights.find(
        (flight) => flight.guid === action.payload
      );
      state.loading = false;
    },
    addFlightFromListeners: (state, action) => {
      state.flights = [...state.flights, action.payload];
      state.lastUpdatedId = action.payload.guid; 

      state.loading = false;
    },
    removeFlightFromListeners: (state, action) => {
      state.flights = state.flights.filter(
        (flight) => flight.flightNumber != action.payload
      );
      state.loading = false;
    },
    updateFlightFromLisiter: (state, action) => {
      state.flights = state.flights.map((flight) =>
        flight.guid === action.payload.guid ? { ...action.payload } : flight
      );
      state.lastUpdatedId = action.payload.guid; // שמירת ה־ID של השורה שעודכנה

      state.loading = false;
    },
    setLoading: (state) => {
      state.loading = true;
    },
    clearFiltered: (state) => {
      state.filteredFlights = [];
      state.loading = false;
      state.error = null
    },
    setMessageError:(state,action) => {
      state.error = action.payload
      state.loading = false
    },
    setLastUpdatedId: (state, action) => {
  state.lastUpdatedId = action.payload;
},
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchAllFlights.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchAllFlights.fulfilled, (state, action) => {
        if (
          action.payload.Header.ReturnCode !== eErrorType.SUCCESS.toString()
        ) {
          state.loading = false;
          state.error = action.payload.Header.ReturnCodeMessage;
        } else {
          state.flights = action.payload.Body;
          state.loading = false;
          state.error = null;
        }
      })
      .addCase(fetchAllFlights.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message ?? "Error loading flights";
      })

      .addCase(fetchFlightById.fulfilled, (state, action) => {
        state.selectedFlight = action.payload;
      })

      .addCase(addFlight.fulfilled, (state, action) => {
                if (
          action.payload.Header.ReturnCode !== eErrorType.SUCCESS.toString()
        ) {
          state.loading = false;
          state.error = action.payload.Header.ReturnCodeMessage;
        } else {
          state.loading = false;
          state.error = null;
        }
        //state.flights.push(action.payload);
      })

      .addCase(editFlight.fulfilled, (state, action) => {
        const index = state.flights.findIndex(
          (f) => f.guid === action.payload.guid
        );
        if (index !== -1) state.flights[index] = action.payload;
      })
      .addCase(editFlight.rejected, (state, action) => {
        state.error =  "ERRROR"
      })
      .addCase(editFlight.pending, (state, action) => {

      })
      .addCase(removeFlight.fulfilled, (state, action) => {
        state.flights = state.flights.filter((f) => f.guid !== action.payload);
      })
      .addCase(getFlightByQuery.pending, (state) => {
        state.loading = true;
      })
      .addCase(getFlightByQuery.fulfilled, (state, action) => {
        if (
          action.payload.Header.ReturnCode !== eErrorType.SUCCESS.toString()
        ) {
          state.loading = false;
          state.error = action.payload.Header.ReturnCodeMessage;
        } else {
          state.filteredFlights = action.payload.Body;
          state.loading = false;
          state.error = null;
        }
      })
      .addCase(getFlightByQuery.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message ?? "Error loading flights";
      });
  },
});
export const {
  addFlightFromListeners,
  setSelectedFlight,
  setlClearSelectedFlight,
  removeFlightFromListeners,
  updateFlightFromLisiter,
  setLoading,
  clearFiltered,
  setMessageError,
  setLastUpdatedId
} = flightSlice.actions;

export default flightSlice.reducer;
