import { createSlice } from "@reduxjs/toolkit";

const unitSlice = createSlice({
    name: "unit",
    initialState: {
        selectedUnit: sessionStorage.getItem("unitId") || "",
    },
    reducers: {
        setUnit: (state, action) => {
            state.selectedUnit = action.payload;
            sessionStorage.setItem("unitId", action.payload); // keep synced
        },
    },
});

export const { setUnit } = unitSlice.actions;
export default unitSlice.reducer;
