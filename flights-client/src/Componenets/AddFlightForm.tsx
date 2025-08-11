import React, { useEffect, useState } from 'react';
import {
  TextField,
  Button,
  Paper,
  Typography,
  Grid
} from '@mui/material';
import { useFlight } from '../UseHook/useFlight';
import { v4 as uuidv4 } from 'uuid';
import { FlightResponse,Flight} from '../Types/FlightTypes';

interface FlightFrom {
  selectedFlight: FlightResponse | null | undefined;
  clearSelection: () => void;
  setLoadingToUpdate:() => void,
  handleValidation: (flight: Flight) => string | null;
}

const AddFlightForm: React.FC<FlightFrom> = ({ selectedFlight, clearSelection,setLoadingToUpdate,handleValidation }) => {
  const { createFlight, updateFlight } = useFlight();
  const [errorMsg, setErrorMsg] = useState<string | null>(null);

  const [form, setForm] = useState({
    guid: '',
    flightNumber: '',
    destination: '',
    departureTime: '',
    gate: '',
    status: ''
  });

  useEffect(() => {
    if (selectedFlight) {
      setForm({ ...selectedFlight });
    } else {
      resetForm();
    }
  }, [selectedFlight]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const resetForm = () => {
    setForm({
      guid: '',
      flightNumber: '',
      destination: '',
      departureTime: '',
      gate: '',
      status: ''
    });
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    const flight: FlightResponse = {
      ...form,
      guid: form.guid || uuidv4(),
      status: form.status,
      createDate: selectedFlight?.createDate || new Date().toISOString(),
    };
    const validationError = handleValidation(flight);
    if (validationError && !selectedFlight) {
      setErrorMsg(validationError);
      return;
    }


    setLoadingToUpdate()
    if (selectedFlight) {
      updateFlight(flight);
    } else {
      createFlight(flight);
    }
    setErrorMsg('')
    resetForm();
    clearSelection();
  };

  return (

    <Paper elevation={3} sx={{ padding: 3, marginBottom: 4 }}>
      <Typography variant="h6" gutterBottom>
        {selectedFlight ? 'Edit Flight' : 'Add New Flight'}
      </Typography>
      <form onSubmit={handleSubmit}>
        <Grid container spacing={2}>
          <Grid component="div">
            <TextField
              label="Flight Number"
              name="flightNumber"
              value={form.flightNumber}
              onChange={handleChange}
              fullWidth
            />
          </Grid>
          <Grid component="div">
            <TextField
              label="Destination"
              name="destination"
              value={form.destination}
              onChange={handleChange}
              fullWidth
            />
          </Grid>
          <Grid component="div">
            <TextField
              label="Departure Time"
              name="departureTime"
              type="datetime-local"
              value={form.departureTime}
              onChange={handleChange}
              InputLabelProps={{ shrink: true }}
              fullWidth
            />
          </Grid>
          <Grid component="div">
            <TextField
              label="Gate"
              name="gate"
              value={form.gate}
              onChange={handleChange}
              fullWidth
            />
          </Grid>
          <Grid component="div">
            <Button type="submit" variant="contained" color="primary">
              {selectedFlight ? 'Save Changes' : 'Add Flight'}
            </Button>
            {selectedFlight && (
              <Button onClick={() => { clearSelection(); resetForm(); }} sx={{ ml: 2 }}>
                Cancel Editing
              </Button>
            )}
          </Grid>
        </Grid>
      </form>
         {errorMsg && <p style={{ color: 'red' }}>{errorMsg}</p>}

    </Paper>
  );
};

export default AddFlightForm;
