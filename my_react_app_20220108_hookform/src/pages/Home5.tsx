import React from 'react';
import Select from "react-select";
import { useForm, Controller, SubmitHandler } from "react-hook-form";
import {List, ListItem, Input} from '@mui/material';
import ResponsiveDrawer from '../components/layout/ResponsiveDrawer';

interface IFormInput {
    firstName: string;
    lastName: string;
    iceCreamType: {label: string; value: string };
  }

const Home5 = () => {
    const { control, handleSubmit } = useForm<IFormInput>();
    const onSubmit: SubmitHandler<IFormInput> = (data) => {
      console.log(data)
    };
  
  return (
    <ResponsiveDrawer>
      <div>
        <form onSubmit={handleSubmit(onSubmit)}>
          <List>
            <ListItem>
              <label>First Name</label>
              <Controller
                name="firstName"
                control={control}
                defaultValue=""
                render={({ field }) => <Input {...field} />}
              />
            </ListItem><ListItem>
              <label>Age</label>
              <Controller
                name="iceCreamType"
                control={control}
                render={({ field }) => <Select 
                  {...field} 
                  options={[
                    { value: "chocolate", label: "Chocolate" },
                    { value: "strawberry", label: "Strawberry" },
                    { value: "vanilla", label: "Vanilla" }
                  ]} 
                />}
              />
            </ListItem><ListItem>
              <input type="submit" />
            </ListItem>
          </List>
        </form>
      </div>
    </ResponsiveDrawer>
  );
}
  
export default Home5;