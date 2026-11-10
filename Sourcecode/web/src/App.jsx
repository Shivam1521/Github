import "./index.scss";
import "./App.css";
import Login from "./pages/Login";
import { Dashboard } from "./pages/Dashboard";
import { Navigate, Outlet, Route, Routes } from "react-router-dom";
import DashboardLayout from "./components/DashboardLayout";
import { Toaster } from "react-hot-toast";
import { EmployeeMaster } from "./pages/masters/EmployeeMaster";
import { RoleMaster } from "./pages/masters/RoleMaster";
import { ItemMaster } from "./pages/masters/ItemMaster";
import { ListMaster } from "./pages/masters/ListMaster";
import { PrintingReport } from "./pages/reports/PrintingReport";
import { MoveOrderReport } from "./pages/reports/MoveOrderReport";
import { Printing } from "./pages/Printing";
import { DataReport } from "./pages/reports/DataReport";
import { MappingReport } from "./pages/reports/MappingReport";
import { DamageReport } from "./pages/reports/DamageReport";
import { TrackingReport } from "./pages/reports/TrackingReport";
import { useState } from "react";
import { UnitMaster } from "./pages/masters/UnitMaster";
import { WarehouseMaster } from "./pages/masters/WarehouseMaster";
import { ProductAuthentication } from "./pages/ProductAuthentication";
import { PeripheralDeviceMaster } from "./pages/masters/PeripheralDeviceMaster";
import { ChangePassword } from "./pages/ChangePassword";
import { ScratchGeneration } from "./pages/ScratchGeneration";
import { PickSlipReceiptStatus } from "./pages/reports/PickSlipReceiptStatus";
import { PickedSlipReport } from "./pages/reports/PickedSlipReport";
import { DashboardNew } from "./pages/DashboardNew";
import { Dashboard3 } from "./pages/dashboards/Dashboard3";
import { DeleteAccount } from "./pages/DeleteAccount";

function App() {
  const [isLoggedIn, setIsLoggedIn] = useState(() => {
    const storedToken = sessionStorage.getItem("token");
    const refreshToken = sessionStorage.getItem("refreshToken");
    return storedToken && refreshToken ? true : false;
  });
  const PrivateRoute = ({ isLoggedIn }) => {
    return isLoggedIn ? <Outlet /> : <Navigate to="/login" replace />;
  };
  return (
    <>
      <Toaster position="top-right" />
      <Routes>
        <Route path="/pa" element={<ProductAuthentication />} />
        <Route
          path="/login"
          element={<Login setIsLoggedIn={setIsLoggedIn} />}
        />
        <Route path="/delete-account-data" element={<DeleteAccount />} />
        <Route element={<PrivateRoute isLoggedIn={isLoggedIn} />}>
          <Route path="/" element={<DashboardLayout />}>
            <Route index element={<Navigate to="/login" replace />} />
            {/* <Route path="dashboard" element={<DashboardNew />} /> */}
              <Route path="dashboard" element={<Dashboard3 />} />
            <Route path="change-password" element={<ChangePassword />} />
            <Route
              path="masters/employee-master"
              element={<EmployeeMaster />}
            />
            <Route path="masters/role-master" element={<RoleMaster />} />
            <Route path="masters/item-master" element={<ItemMaster />} />
            <Route path="masters/list-master" element={<ListMaster />} />
            <Route
              path="masters/peripheral-device-master"
              element={<PeripheralDeviceMaster />}
            />
            <Route path="masters/unit-master" element={<UnitMaster />} />
            <Route
              path="masters/warehouse-master"
              element={<WarehouseMaster />}
            />
            <Route path="printing" element={<Printing />} />
            <Route
              path="transactions/scratch-generation"
              element={<ScratchGeneration />}
            />

            <Route path="reports/data-report" element={<DataReport />} />
            <Route
              path="reports/printing-report"
              element={<PrintingReport />}
            />
            <Route path="reports/mapping-report" element={<MappingReport />} />
            <Route path="reports/damage-report" element={<DamageReport />} />
            <Route
              path="reports/tracking-report"
              element={<TrackingReport />}
            />
            <Route
              path="reports/move-order-report"
              element={<MoveOrderReport />}
            />
            <Route
              path="reports/pick-slip-receipt-status"
              element={<PickSlipReceiptStatus />}
            />
            <Route
              path="reports/picked-slip-report"
              element={<PickedSlipReport />}
            />
          </Route>
        </Route>
        <Route path="*" element={<Navigate to="/login" replace />} />
      </Routes>
    </>
  );
}

export default App;
