import { useState } from "react";
import {
  TextField,
  Button,
  Grid,
  MenuItem,
  Select,
  InputLabel,
  FormControl,
  Paper,
  Typography,
} from "@mui/material";
import { SelectChangeEvent } from '@mui/material/Select';

export enum StatusFlight {
  Boarding = "Boarding",
  Departed = "Departed",
  Landed = "Landed",
  Scheduled = "Scheduled",
}

interface SearchFlightProps {
  onSearch: (query:string) => void;
  clearFilteredFlight: () => void;
}

const SearchFlight: React.FC<SearchFlightProps> = ({ onSearch, clearFilteredFlight }) => {
  const [form, setForm] = useState({
    destination: "",
    status: "",
  });

const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
  const { name, value } = e.target;
  setForm((prev) => ({ ...prev, [name]: value }));
};

const handleSelectChange = (e: SelectChangeEvent) => {
  const { name, value } = e.target;
  setForm((prev) => ({ ...prev, [name!]: value }));
};
  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if(form.destination.trim().length===0 && form.status.trim().length===0) return
    if(form.status == 'all') {
      onSearch ("ALL_DATA")
      return
    }
    var query = `${form.destination ? `destination=${form.destination}` : ''}${form.status ? `${form.destination ? `&` : ''}status=${form.status}` : ''}`;
    onSearch(query);
  };
  const handleClear = () => {
    setForm({ status: "", destination: "" });
    clearFilteredFlight();
  };

  return (
    <Paper elevation={3} sx={{ padding: 3, marginBottom: 4 }}>
      <Typography variant="h6" gutterBottom>
       Search Flight
      </Typography>
      <form onSubmit={handleSubmit}>
        <Grid container spacing={2} alignItems="center">
          <Grid component="div">
            <TextField
              label="Destination"
              name="destination"
              value={form.destination}
          onChange={handleInputChange}
              fullWidth
            />
          </Grid>

          <Grid component="div">
<FormControl fullWidth size="medium" sx={{ minWidth: 120 }}>
  <InputLabel id="status-label">Status</InputLabel>
  <Select
    labelId="status-label"
    name="status"
    value={form.status}
    label="status"
    onChange={handleSelectChange}
    fullWidth
    sx={{ height: '56px' }} // גובה ברירת מחדל של TextField
  >
    <MenuItem value="all">All</MenuItem>
    {Object.values(StatusFlight).map((status) => (
      <MenuItem key={status} value={status}>
        {status}
      </MenuItem>
    ))}
  </Select>
</FormControl>
          </Grid>

          <Grid component="div">
            <Button type="submit" variant="contained" color="primary" fullWidth>
              Search
            </Button>
          </Grid>
          <Grid component="div">
            <Button
              variant="outlined"
              color="secondary"
              fullWidth
              onClick={handleClear}
            >
              Clear
            </Button>
          </Grid>
        </Grid>
      </form>
    </Paper>
  );
};

export default SearchFlight;
