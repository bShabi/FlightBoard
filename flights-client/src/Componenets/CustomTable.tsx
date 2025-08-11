// CustomTable.tsx
import React, { useEffect } from 'react';
import {
  Table, TableBody, TableCell, TableContainer, TableHead, TableRow,
  Paper, IconButton, TableSortLabel, Typography, CircularProgress, Box
} from '@mui/material';
import VisibilityIcon from '@mui/icons-material/Visibility';
import DeleteIcon from '@mui/icons-material/Delete';
import { motion } from 'framer-motion';

export interface Column {
  id: string;
  label: string;
}

interface CustomTableProps {
  data: any[];
  isLoading: boolean;                 
  columns: Column[];
  filterColumn: string;
  onHeaderClick?: (columnId: string) => void;
  onHandleClick: (row: any) => void;
  onHandleRemoveClick: (row: any) => void;

  rowIdField?: string;                
  highlightedId?: string | null;      
  onHighlightConsumed?: () => void;   
}

const MotionTableRow = motion(TableRow);

const CustomTable: React.FC<CustomTableProps> = ({
  data,
  isLoading,
  columns,
  filterColumn,
  onHeaderClick,
  onHandleClick,
  onHandleRemoveClick,
  rowIdField = 'guid',
  highlightedId = null,
  onHighlightConsumed,
}) => {
  useEffect(() => {
    console.log('🔄 Data has been updated:', data);
  }, [data]);

  const formatDateTimeDisplay = (dateTimeStr: string): string => {
    const date = new Date(dateTimeStr);
    const day = String(date.getDate()).padStart(2, '0');
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const year = date.getFullYear();
    const hours = String(date.getHours()).padStart(2, '0');
    const minutes = String(date.getMinutes()).padStart(2, '0');
    return `${day}-${month}-${year} ${hours}:${minutes}`;
  };

  if (isLoading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', py: 3 }}>
        <CircularProgress />
      </Box>
    );
  }

  return (
    <TableContainer component={Paper} sx={{ maxHeight: 400, overflow: 'auto' }}>
      <Table stickyHeader>
        <TableHead>
          <TableRow>
            <TableCell>#</TableCell>
            {columns.map((col) => (
              <TableCell key={col.id}>
                <TableSortLabel
                  active={filterColumn === col.id}
                  direction="asc"
                  onClick={() => onHeaderClick?.(col.id)}
                >
                  {col.label}
                </TableSortLabel>
              </TableCell>
            ))}
            <TableCell align="center">צפייה</TableCell>
            <TableCell align="center">מחיקה</TableCell>
          </TableRow>
        </TableHead>

        <TableBody>
          {data.length > 0 ? (
            data.map((row, idx) => {
              const id = row[rowIdField] ?? idx;
              const isHighlighted = highlightedId !== null && id === highlightedId;

              return (
                <MotionTableRow
                  key={id}
                  layout
                  hover
                  initial={{ opacity: 0, y: 8 }}
                  animate={{
                    opacity: 1,
                    y: 0,
                    backgroundColor: isHighlighted ? ['green', '#ffffff'] : undefined,
                  }}
                  transition={{ duration: 0.6 }}
                  onAnimationComplete={() => {
                    if (isHighlighted) onHighlightConsumed?.();
                  }}
                >
                  <TableCell>{idx + 1}</TableCell>

                  {columns.map((col) => (
                    <TableCell key={col.id}>
                      {col.id === 'departureTime'
                        ? formatDateTimeDisplay(row[col.id])
                        : row[col.id]}
                    </TableCell>
                  ))}

                  <TableCell align="center">
                    <IconButton color="primary" onClick={() => onHandleClick(row)} aria-label="View">
                      <VisibilityIcon />
                    </IconButton>
                  </TableCell>
                  <TableCell align="center">
                    <IconButton color="error" onClick={() => onHandleRemoveClick(row)} aria-label="Delete">
                      <DeleteIcon />
                    </IconButton>
                  </TableCell>
                </MotionTableRow>
              );
            })
          ) : (
            <TableRow>
              <TableCell colSpan={columns.length + 3} align="center">
                <Typography color="text.secondary">No data found</Typography>
              </TableCell>
            </TableRow>
          )}
        </TableBody>
      </Table>
    </TableContainer>
  );
};

export default CustomTable;
